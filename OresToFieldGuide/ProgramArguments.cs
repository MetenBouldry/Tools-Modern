﻿using System.Text;

namespace OresToFieldGuide
{
    /// <summary>
    /// Represents the arguments used by the program
    /// </summary>
    internal struct ProgramArguments
    {
        /// <summary>
        /// The path to the .minecraft folder
        /// </summary>
        public string ModpackFolder;

        /// <summary>
        /// The path to the Language Token JSON files
        /// </summary>
        public string DataFolder;

        /// <summary>
        /// Any PatchouliEntry FileName specified here will not be deleted from the game's field guide.
        /// </summary>
        public string[] WhitelistedPatchouliEntryFilenames;
    }
}
