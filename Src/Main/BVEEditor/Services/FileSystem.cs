using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Services
{
    sealed class FileSystem : IFileSystem
    {
        readonly IPropertyService property_service;

        public FileSystem(IPropertyService propertyService)
        {
            property_service = propertyService;
        }

        #region IFileSystem メンバー

        public void Delete(FileName path)
        {
            try{
                File.Delete(path);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public void CopyFile(FileName source, FileName destination, bool overwrite = false)
        {
            try{
                File.Copy(source, destination, overwrite);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public void CreateDirectory(DirectoryName path)
        {
            try {
                Directory.CreateDirectory(path);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public Stream OpenWrite(FileName fileName)
        {
            try{
                return File.OpenWrite(fileName);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public void WriteAllText(FileName path, string content)
        {
            try{
                File.WriteAllText(path, content);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public void AppendAllText(FileName path, string content)
        {
            try{
                File.AppendAllText(path, content);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        #endregion

        #region IReadOnlyFileSystem メンバー

        public int DefaultFileEncodingCodePage{
            get{return property_service.Get("BVEEditor.DefaultFileEncoding", 65001);}
            set{property_service.Set("BVEEditor.DefaultFileEncoding", value);}
        }

        public Encoding DefaultFileEncoding{
            get{return Encoding.GetEncoding(DefaultFileEncodingCodePage);}
        }

        readonly EncodingInfo[] all_encodings = Encoding.GetEncodings().OrderBy(e => e.DisplayName).ToArray();

        public IReadOnlyList<EncodingInfo> AllEncodings{
            get{return all_encodings;}
        }

        public EncodingInfo DefaultFileEncodingInfo{
            get{
                int cp = DefaultFileEncodingCodePage;
                return all_encodings.Single(e => e.CodePage == cp);
            }
            set{
                DefaultFileEncodingCodePage = value.CodePage;
            }
        }

        public bool FileExists(FileName path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(DirectoryName path)
        {
            return Directory.Exists(path);
        }

        public Stream OpenRead(FileName fileName)
        {
            try{
                return File.OpenRead(fileName);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public TextReader OpenText(FileName fileName)
        {
            try{
                return File.OpenText(fileName);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        public IEnumerable<FileName> GetFiles(DirectoryName directory, string searchPattern = "*",
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try{
                return Directory.EnumerateFiles(directory, searchPattern, searchOption).Select(FileName.Create);
            }
            catch(UnauthorizedAccessException e){
                throw new IOException(e.Message, e);
            }
        }

        #endregion
    }
}
