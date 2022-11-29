//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace WebApplication.Utilities
{
    public static class Json
    {
        /// <summary>
        /// Serialize data as JSON to a stream.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writeIndented">Write indented JSON.</param>
        /// <returns>The stream. Should be disposed.</returns>
        public static MemoryStream ToStream<T>(T data, bool writeIndented = false)
        {
            var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream: memoryStream, encoding: Encoding.UTF8, bufferSize: 4096, leaveOpen: true)) // last parameter is important
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, data);
                streamWriter.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
            }
            return memoryStream;

        }



        /// <summary>
        /// Deserialize JSON file content.
        /// </summary>
        public static T DeserializeFile<T>(string filename)
        {
            var content = File.ReadAllText(filename, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}

