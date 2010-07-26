using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Used to ease interop with the COM debugging extension.
    /// </summary>
    internal static class COMHelpers
    {
        /// <summary>
        /// Gets the language type GUID for the symbol store.
        /// </summary>
        public static readonly Guid LanguageType = System.Diagnostics.SymbolStore.SymLanguageType.JScript;

        /// <summary>
        /// Gets the language vendor GUID for the symbol store.
        /// </summary>
        public static readonly Guid LanguageVendor =    // CFA05A92-B7CC-4D3D-92E1-4D18CDACDC8D
            new Guid(0xCFA05A92, 0xB7CC, 0x4D3D, 0x92, 0xE1, 0x4D, 0x18, 0xCD, 0xAC, 0xDC, 0x8D);
        

        /// <summary>
        /// Gets the document type GUID for the symbol store.
        /// </summary>
        public static readonly Guid DocumentType = System.Diagnostics.SymbolStore.SymDocumentType.Text;
    }
}
