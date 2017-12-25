/* 
* 2017 Microsoft Corp
* 
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS”
* AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
* FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
* HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
* OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
* OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#region Information

// Solution:  HL7V2Connector
// HL7V2Connector
// File:  Parser.cs
// 
// Created: 09/06/2017 : 3:15 PM
// 
// Modified By: Howard Edidin
// Modified:  09/06/2017 : 3:16 PM

#endregion

namespace HL7V2Parser
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    public class Parser
    {

        private readonly int[] _defaultSegmentDelims = {'\r', '\n'};
        private readonly string[] _headerSegments = {"MSH", "FHS", "FTS", "BHS", "BTS"};
        private readonly string[] _headerSegmentsWithDelimiters = {"MSH", "FHS", "BHS"};



        /// <summary>
        /// Parses the specified HL7.
        /// </summary>
        /// <param name="hl7">The HL7.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">hl7</exception>
        public XDocument Parse(string hl7)
        {
            if (hl7 == null)
                throw new ArgumentNullException(nameof(hl7));

            using (var reader = new StringReader(hl7))
            {
                return Parse(reader);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public XDocument Parse(TextReader reader)
        {
            var startSegment = string.Concat((char) reader.Read(), (char) reader.Read(), (char) reader.Read());

            //is this a HL7 message?
            if (startSegment != "MSH" && startSegment != "FHS" && startSegment != "BHS")
                throw new XmlException("Not a valid HL7. (HL7 must start with MSH, FHS or BHS)");

            //get the delimiters
            var fieldDelim = reader.Read();
            var componentDelim = reader.Read();
            var repetitionSep = reader.Read();
            var escapeChar = reader.Read();
            var subcomponentDelim = reader.Read();

            var doc = new XDocument(
                new XElement("HL7")
            );

            XElement field = null;
            XElement component = null;
            XElement subComponent = null;
            var fieldIndex = 1;
            var componentIndex = 1;
            var subComponentIndex = 1;
            var isHeader = true;
            var isHeaderWithDelimiters = true;
            string token = null;
            var beginEscapeChar = false;

            // Get the first data element from the file.
            var val = new TokenDelim(startSegment, fieldDelim);
            var segmentName = val.Token;
            var segment = new XElement(segmentName);
            if (doc.Root != null)
            {
                doc.Root.Add(segment);

                field = new XElement(segmentName + "." + fieldIndex, (char) fieldDelim);
                segment.Add(field);
                fieldIndex++;
                field = new XElement(segmentName + "." + fieldIndex,
                    string.Concat((char) componentDelim, (char) repetitionSep, (char) escapeChar,
                        (char) subcomponentDelim));
                segment.Add(field);
                fieldIndex++;

                reader.Read();
                val = NextToken(reader, _defaultSegmentDelims, fieldDelim, componentDelim, repetitionSep, escapeChar,
                    subcomponentDelim);

                //create a new element with each data token from //the stream.
                while (val.Delimiter >= 0 && val.Delimiter < short.MaxValue)
                {
                    if (_defaultSegmentDelims.Contains(val.Delimiter))
                    {
                        if (subComponent != null)
                        {
                            subComponent =
                                new XElement(
                                    segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex,
                                    token + val.Token);
                            component.Add(subComponent);
                        }
                        else if (component != null)
                        {
                            component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex,
                                token + val.Token);
                            field.Add(component);
                        }
                        else if (segment != null)
                        {
                            field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                            segment.Add(field);
                        }

                        segment = null;
                        field = null;
                        fieldIndex = 1;
                        component = null;
                        componentIndex = 1;
                        subComponent = null;
                        subComponentIndex = 1;

                        token = "";
                    }
                    else if (val.Delimiter == fieldDelim)
                    {
                        if (segment == null)
                        {
                            segmentName = token + val.Token;
                            segment = new XElement(segmentName);
                            doc.Root.Add(segment);
                            isHeader = _headerSegments.Contains(segmentName);
                            isHeaderWithDelimiters = _headerSegmentsWithDelimiters.Contains(segmentName);

                            if (isHeaderWithDelimiters)
                            {
                                /*fieldDelim*/
                                reader.Read();
                                /*componentDelim*/
                                reader.Read();
                                /*repetitionSep*/
                                reader.Read();
                                /*escapeChar*/
                                reader.Read();
                                /*subcomponentDelim*/
                                reader.Read();

                                field = new XElement(segmentName + "." + fieldIndex, (char) fieldDelim);
                                segment.Add(field);
                                fieldIndex++;
                                field = new XElement(segmentName + "." + fieldIndex,
                                    string.Concat((char) componentDelim, (char) repetitionSep, (char) escapeChar,
                                        (char) subcomponentDelim));
                                segment.Add(field);
                            }
                            else
                            {
                                fieldIndex--;
                            }
                        }
                        else
                        {
                            if (fieldIndex > 0)
                                if (isHeaderWithDelimiters && fieldIndex == 1)
                                {
                                    field = new XElement(segmentName + "." + fieldIndex,
                                        string.Concat((char) componentDelim, (char) repetitionSep, (char) escapeChar,
                                            (char) subcomponentDelim));
                                    segment.Add(field);
                                }
                                else if (componentIndex == 1)
                                {
                                    field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                                    segment.Add(field);
                                }
                                else
                                {
                                    component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex,
                                        token + val.Token);
                                    field.Add(component);
                                }
                            component = null;
                            componentIndex = 1;
                            subComponent = null;
                            subComponentIndex = 1;
                        }
                        fieldIndex++;
                        token = "";
                    }
                    else if (val.Delimiter == componentDelim)
                    {
                        if (!isHeader || fieldIndex != 1)
                        {
                            if (componentIndex == 1)
                            {
                                field = new XElement(segmentName + "." + fieldIndex);
                                segment.Add(field);
                            }

                            if (subComponentIndex == 1)
                            {
                                component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex,
                                    token + val.Token);
                                field.Add(component);
                            }
                            else
                            {
                                subComponent =
                                    new XElement(
                                        segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex,
                                        token + val.Token);
                                component.Add(subComponent);
                            }

                            componentIndex++;
                            subComponent = null;
                            subComponentIndex = 1;
                            token = "";
                        }
                    }
                    else if (val.Delimiter == repetitionSep)
                    {
                        if (!isHeader || fieldIndex != 1)
                        {
                            component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex,
                                token + val.Token);
                            field.Add(component);

                            componentIndex = 1;
                            subComponentIndex = 1;
                        }
                    }
                    else if (val.Delimiter == escapeChar)
                    {
                        beginEscapeChar = !beginEscapeChar;
                        //\Cxxyy\ 	Single-byte character set escape sequence with two hexadecimal values not converted
                        //\E\ 	    Escape character converted to escape character (e.g., ‘\’)
                        //\F\ 	    Field separator converted to field separator character (e.g., ‘|’)
                        //\H\ 	    Start highlighting not converted
                        //\Mxxyyzz\ Multi-byte character set escape sequence with two or three hexadecimal values (zz is optional) not converted
                        //\N\ 	    Normal text (end highlighting) not converted
                        //\R\ 	    Repetition separator converted to repetition separator character (e.g., ‘~’)
                        //\S\ 	    Component separator converted to component separator character (e.g., ‘^’)
                        //\T\ 	    Subcomponent separator converted to subcomponent separator character (e.g., ‘&’)
                        //\Xdd…\ 	Hexadecimal data (dd must be hexadecimal characters) converted to the characters identified by each pair of digits
                        //\Zdd…\ 	Locally defined escape sequence not converted
                        if (!isHeader || fieldIndex != 1)
                            if (!string.IsNullOrEmpty(val.Token))
                                if (!beginEscapeChar)
                                    if (val.Token.StartsWith("C"))
                                    {
                                    }
                                    else if (val.Token == "E")
                                    {
                                        token += (char) escapeChar;
                                    }
                                    else if (val.Token == "F")
                                    {
                                        token += fieldDelim;
                                    }
                                    else if (val.Token == "H")
                                    {
                                    }
                                    else if (val.Token.StartsWith("M"))
                                    {
                                    }
                                    else if (val.Token == "N")
                                    {
                                    }
                                    else if (val.Token == "R")
                                    {
                                        token += (char) repetitionSep;
                                    }
                                    else if (val.Token == "S")
                                    {
                                        token += (char) componentDelim;
                                    }
                                    else if (val.Token == "T")
                                    {
                                        token += (char) subcomponentDelim;
                                    }
                                    else if (val.Token.StartsWith("X"))
                                    {
                                    }
                                    else if (val.Token.StartsWith("Z"))
                                    {
                                    }
                                    else
                                    {
                                        token += (char) escapeChar;
                                        token += val.Token;
                                        token += (char) escapeChar;
                                    }
                                else
                                    token += val.Token;
                    }
                    else if (val.Delimiter == subcomponentDelim)
                    {
                        if (!isHeader || fieldIndex != 1)
                        {
                            if (subComponentIndex == 1)
                            {
                                component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex);
                                field.Add(component);
                            }

                            subComponent =
                                new XElement(
                                    segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex,
                                    token + val.Token);
                            component.Add(subComponent);
                            subComponentIndex++;
                            token = "";
                        }
                    }

                    val = NextToken(reader, _defaultSegmentDelims, fieldDelim, componentDelim, repetitionSep,
                        escapeChar,
                        subcomponentDelim);
                }
            }

            //add the last token
            if (subComponentIndex > 1 && component != null)
            {
                subComponent =
                    new XElement(segmentName + "." + fieldIndex + "." + componentIndex + "." + subComponentIndex,
                        token + val.Token);
                component.Add(subComponent);
            }
            else if (componentIndex > 1 && field != null)
            {
                component = new XElement(segmentName + "." + fieldIndex + "." + componentIndex, token + val.Token);
                field.Add(component);
            }
            else if (segment != null)
            {
                field = new XElement(segmentName + "." + fieldIndex, token + val.Token);
                segment.Add(field);
            }

            return doc;
        }

        private TokenDelim NextToken(TextReader reader, int[] segmentDelims, int fieldDelim, int componentDelim,
            int repetitionSep, int escapeChar, int subcomponentDelim)
        {
            var token = "";
            var temp = reader.Read();
            while (temp != -1 && !segmentDelims.Contains(temp) && temp != fieldDelim && temp != componentDelim &&
                   temp != repetitionSep && temp != escapeChar && temp != subcomponentDelim)
            {
                token += (char) temp;
                temp = reader.Read();
            }
            return new TokenDelim(token, temp);
        }

        private class TokenDelim
        {
            public TokenDelim(string token, int delimiter)
            {
                Token = token;
                Delimiter = delimiter;
            }

            public string Token { get; }
            public int Delimiter { get; }
        }
    }
}
