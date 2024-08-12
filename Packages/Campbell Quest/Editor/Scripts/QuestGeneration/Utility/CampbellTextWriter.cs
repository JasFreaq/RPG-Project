using System.IO;
using System.Text;

namespace Campbell.Editor.QuestGeneration.Utility
{
    public class CampbellTextWriter : TextWriter
    {
        private StringWriter _stringWriter;

        public CampbellTextWriter(StringWriter stringWriter)
        {
            _stringWriter = stringWriter;
        }

        public override Encoding Encoding => _stringWriter.Encoding;

        public override void Write(char value)
        {
            _stringWriter.Write(value);
        }

        public override void WriteLine(string value)
        {
            _stringWriter.WriteLine(value);
        }

        // Implement the write method for the Python runtime
        public void write(string value)
        {
            _stringWriter.Write(value);
        }
    }
}