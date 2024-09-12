using System.IO;
using System.Text;

namespace Campbell.Editor.QuestGeneration.Utility
{
    /// <summary>
    /// A custom text writer that extends the TextWriter class.
    /// It wraps a StringWriter to provide additional functionalities.
    /// </summary>
    public class CampbellTextWriter : TextWriter
    {
        private StringWriter _stringWriter;

        /// <summary>
        /// Initializes a new instance of the CampbellTextWriter class with a specified StringWriter.
        /// </summary>
        /// <param name="stringWriter">The StringWriter instance to be wrapped by this text writer.</param>
        public CampbellTextWriter(StringWriter stringWriter)
        {
            _stringWriter = stringWriter;
        }

        /// <summary>
        /// Gets the encoding of the underlying StringWriter.
        /// </summary>
        public override Encoding Encoding => _stringWriter.Encoding;

        /// <summary>
        /// Writes a single character to the underlying StringWriter.
        /// </summary>
        /// <param name="value">The character to write.</param>
        public override void Write(char value)
        {
            _stringWriter.Write(value);
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the underlying StringWriter.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void WriteLine(string value)
        {
            _stringWriter.WriteLine(value);
        }

        /// <summary>
        /// Writes a string to the underlying StringWriter.
        /// This method is intended for compatibility with the Python runtime.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public void write(string value)
        {
            _stringWriter.Write(value);
        }
    }
}