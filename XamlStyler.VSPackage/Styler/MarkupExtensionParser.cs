﻿namespace XamlStyler.XamlStylerVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using XamlStyler.XamlStylerVSPackage.StylerModels;

    public static class MarkupExtensionParser
    {
        #region Fields

        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");

        #endregion Fields

        #region Methods

        public static MarkupExtensionInfo Parse(string input)
        {
            #region Parameter Checks

            if (!MarkupExtensionPattern.IsMatch(input))
            {
                string msg = String.Format("{0} is not a MarkupExtension.", input);
                throw new InvalidOperationException(msg);
            }

            #endregion

            MarkupExtensionInfo resultInfo = new MarkupExtensionInfo();

            using (StringReader reader = new StringReader(input))
            {
                MarkupExtensionParsingModeEnum parsingMode = MarkupExtensionParsingModeEnum.START;

                try
                {
                    //Debug.Print("Parsing '{0}'", input);
                    //Debug.Indent();

                    while (MarkupExtensionParsingModeEnum.END != parsingMode
                        && MarkupExtensionParsingModeEnum.UNEXPECTED != parsingMode)
                    {
                        //Debug.Print(context.ToString());
                        //Debug.Indent();

                        switch (parsingMode)
                        {
                            case MarkupExtensionParsingModeEnum.START:
                                parsingMode = reader.ReadMarkupExtensionStart(resultInfo);
                                break;

                            case MarkupExtensionParsingModeEnum.MARKUP_NAME:
                                parsingMode = reader.ReadMarkupName(resultInfo);
                                break;

                            case MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR:
                                parsingMode = reader.ReadNameValuePair(resultInfo);
                                break;

                            default:
                                // Do nothing
                                break;
                        }

                        //Debug.Unindent();
                    }
                }
                catch (Exception exp)
                {
                    throw new InvalidDataException(
                            String.Format("Cannot parse markup extension string:\r\n \"{0}\"", input), exp);
                }
                finally
                {
                    //Debug.Unindent();
                }
            }

            return resultInfo;
        }

        private static bool IsEnd(this StringReader reader)
        {
            return (reader.Peek() < 0);
        }

        private static char PeekChar(this StringReader reader)
        {
            char result = (char)reader.Peek();

            //Debug.Print("?Peek '{0}'", result);

            return result;
        }

        private static char ReadChar(this StringReader reader)
        {
            char result = (char)reader.Read();

            //Debug.Print("!Read '{0}'", result);

            return result;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupExtensionStart(this StringReader reader, MarkupExtensionInfo info)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            reader.SeekTill(x => '{' != x && !Char.IsWhiteSpace(x));

            return MarkupExtensionParsingModeEnum.MARKUP_NAME;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupName(this StringReader reader, MarkupExtensionInfo info)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            char[] stopChars = { ' ', '}' };
            MarkupExtensionParsingModeEnum resultParsingMode = MarkupExtensionParsingModeEnum.UNEXPECTED;
            StringBuilder buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                char c = reader.ReadChar();

                if (stopChars.Contains(c))
                {
                    switch (c)
                    {
                        case ' ':
                            resultParsingMode = MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR;
                            break;

                        case '}':
                            resultParsingMode = MarkupExtensionParsingModeEnum.END;
                            break;

                        default:
                            throw new InvalidDataException(
                                String.Format("[{0}] Should not encounter '{1}'.", methodName, c));
                    }

                    info.Name = buffer.ToString().Trim();
                    buffer.Clear();

                    // break out the while
                    break;
                }
                else
                {
                    buffer.Append(c);
                }
            }

            if (MarkupExtensionParsingModeEnum.UNEXPECTED == resultParsingMode)
            {
                throw new InvalidDataException(
                    String.Format("[{0}] Invalid result context: {1}", methodName, resultParsingMode));
            }

            return resultParsingMode;
        }

        private static MarkupExtensionParsingModeEnum ReadNameValuePair(this StringReader reader, MarkupExtensionInfo info)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            char[] stopChars = { ',', '=', '}' };

            MarkupExtensionParsingModeEnum resultParsingMode = MarkupExtensionParsingModeEnum.UNEXPECTED;
            string key = null;
            object value = null;

            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            // When '{' is the starting char, the following must be a value instead of a key.
            //
            // E.g.,
            //    <Setter x:Uid="Setter_75"
            //            Property="Foreground"
            //            Value="{DynamicResource {x:Static SystemColors.ControlTextBrushKey}}" />
            //
            // In other words, "key" shall not start with '{', as it won't be a valid property name.
            if ('{' != reader.PeekChar())
            {
                string temp = reader.ReadTill(x => stopChars.Contains(x)).Trim();
                char keyValueIndicatorChar = reader.PeekChar();

                switch (keyValueIndicatorChar)
                {
                    case ',':
                    case '}':
                        value = temp;
                        break;

                    case '=':
                        key = temp;
                        // Consume the '='
                        reader.Read();
                        break;

                    default:
                        throw new InvalidDataException(
                                    String.Format("[{0}] Should not encounter '{1}'.", methodName, keyValueIndicatorChar));
                }
            }

            if (null == value)
            {
                reader.SeekTill(x => !(Char.IsWhiteSpace(x)));

                string input = reader.ReadValueString().Trim();

                if (MarkupExtensionPattern.IsMatch(input))
                {
                    value = Parse(input);
                }
                else
                {
                    value = input;
                }
            }

            if (String.IsNullOrEmpty(key))
            {
                info.ValueOnlyProperties.Add(value);
            }
            else
            {
                info.KeyValueProperties.Add(new KeyValuePair<string, object>(key, value));
            }

            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            char stopChar = reader.ReadChar();

            switch (stopChar)
            {
                case ',':
                    resultParsingMode = MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR;
                    break;

                case '}':
                    resultParsingMode = MarkupExtensionParsingModeEnum.END;
                    break;

                default:
                    throw new InvalidDataException(
                        String.Format("[{0}] Should not encounter '{1}'.", methodName, stopChar));
            }

            if (MarkupExtensionParsingModeEnum.UNEXPECTED == resultParsingMode)
            {
                throw new InvalidDataException(
                    String.Format("[{0}] Invalid result context: {1}", methodName, resultParsingMode));
            }

            return resultParsingMode;
        }

        private static string ReadTill(this StringReader reader, Func<char, bool> stopAt)
        {
            StringBuilder buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                if (stopAt((char)reader.Peek()))
                {
                    break;
                }
                else
                {
                    buffer.Append(reader.ReadChar());
                }
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException("[ReadTill] Cannot meet the stop condition.");
            }

            return buffer.ToString();
        }

        private static string ReadValueString(this StringReader reader)
        {
            StringBuilder buffer = new StringBuilder();
            int curlyBracePairCounter = 0;
            MarkupExtensionParsingModeEnum parsingMode = MarkupExtensionParsingModeEnum.UNEXPECTED;

            // ignore leading spaces
            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            #region Determine parsing mode

            char c = reader.ReadChar();
            buffer.Append(c);

            if ('{' == c)
            {
                char peek = reader.PeekChar();
                if ('}' != peek)
                {
                    parsingMode = MarkupExtensionParsingModeEnum.MARKUP_EXTENSION_VALUE;

                }
                else
                {
                    parsingMode = MarkupExtensionParsingModeEnum.LITERAL_VALUE;
                }
                curlyBracePairCounter++;
            }
            else if ('\'' == c)
            {
                parsingMode = MarkupExtensionParsingModeEnum.QUOTED_LITERAL_VALUE;
            }
            else
            {
                parsingMode = MarkupExtensionParsingModeEnum.LITERAL_VALUE;
            }

            #endregion

            switch (parsingMode)
            {
                case MarkupExtensionParsingModeEnum.MARKUP_EXTENSION_VALUE:
                    while (curlyBracePairCounter > 0 && (!reader.IsEnd()))
                    {
                        c = reader.ReadChar();
                        buffer.Append(c);

                        switch (c)
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;

                            case '}':
                                curlyBracePairCounter--;
                                break;

                            default:
                                // Do nothing
                                break;
                        }
                    }
                    break;

                case MarkupExtensionParsingModeEnum.QUOTED_LITERAL_VALUE:
                    // Following case is handled:
                    //      StringFormat='{}{0}\'s email'
                    do
                    {
                        buffer.Append(reader.ReadTill(x => '\'' == x));
                        buffer.Append(reader.ReadChar());
                    } while (buffer.Length > 2 && '\'' == buffer[buffer.Length - 1] && '\\' == buffer[buffer.Length - 2]);

                    break;

                case MarkupExtensionParsingModeEnum.LITERAL_VALUE:
                    bool shouldStop = false;

                    while (!reader.IsEnd())
                    {
                        switch (reader.PeekChar())
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;

                            case '}':
                                if (curlyBracePairCounter > 0)
                                {
                                    curlyBracePairCounter--;
                                }
                                else
                                {
                                    shouldStop = true;
                                }
                                break;

                            case ',':
                                // Following case is handled:
                                //      StringFormat={}{0:##\,#0.00;(##\,#0.00);
                                if ('\\' != buffer[buffer.Length - 1])
                                {
                                    shouldStop = true;
                                }
                                break;

                            default:
                                // Do nothing
                                break;
                        }

                        if (!shouldStop)
                        {
                            buffer.Append(reader.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;

                default:
                    throw new InvalidDataException(
                        String.Format("Should not encouter parsingMode {0}", parsingMode)
                        );
            }

            return buffer.ToString();
        }

        private static void SeekTill(this StringReader reader, Func<char, bool> stopAt)
        {
            while (!reader.IsEnd())
            {
                if (stopAt((char)reader.Peek()))
                {
                    break;
                }
                else
                {
                    reader.ReadChar();
                }
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException("[SeekTill] Cannot meet the stop condition.");
            }
        }

        #endregion Methods
    }
}