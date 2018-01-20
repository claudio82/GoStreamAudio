using System;

namespace GoStreamAudioLib
{
    public interface IAudioFileInspector
    {
        /// <summary>
        /// returns the audio file extension
        /// </summary>
        string FileExtension { get; }
        /// <summary>
        /// returns the audio file type description
        /// </summary>
        string FileTypeDescription { get; }
        /// <summary>
        /// describe the audio file properties
        /// </summary>
        string Describe(string fileName);
    }
}
