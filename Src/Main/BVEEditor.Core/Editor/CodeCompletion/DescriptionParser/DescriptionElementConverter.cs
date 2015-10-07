using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// The description parser parses an input string and replaces all tags with rich text contents.
    /// </summary>
    public class DescriptionElementConverter : IValueConverter
    {
        static Regex TagSearcher = new Regex(@"<([a-zA-Z]+)(.+?)>(\w+)</([a-zA-Z]+)>", RegexOptions.Compiled);
        static Regex ParamSearcher = new Regex("([a-zA-Z]+)\\s*=\\s*\"([\\w0-9]+)\"", RegexOptions.Compiled);

        Dictionary<string, IDescriptionElementTransformer> transformers = new Dictionary<string, IDescriptionElementTransformer>{
            {"param", new ParamDescriptionElementTransformer()}
        };

        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value as string;
            if(str == null)
                throw new Exception("");

            TextBlock block;
            if(Parse(str, out block)){
                return block;
            }else{
                block = new TextBlock();
                var split = str.Split('\n');
                foreach(var line in split){
                    block.Inlines.Add(new Run(line));
                    block.Inlines.Add(new LineBreak());
                }
                return block;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        bool Parse(string source, out TextBlock parsed)
        {
            bool result = false;
            var block = new TextBlock();
            for(int i = 0; i < source.Length;){
                var match1 = TagSearcher.Match(source, i);
                if(match1.Success){
                    if(!result){
                        var before_tag = source.Substring(0, match1.Index);
                        var lines = before_tag.Split('\n');
                        foreach(var line in lines){
                            if(!string.IsNullOrEmpty(line))
                                block.Inlines.Add(new Span(new Run(line)));
                            
                            block.Inlines.Add(new LineBreak());
                        }
                        i += match1.Index;
                        result = true;
                    }

                    var open_tag = match1.Groups[1];
                    var close_tag = match1.Groups[4];
                    if(!open_tag.Value.Equals(close_tag.Value))
                        throw new Exception("Expected a close tag of " + open_tag.Value + " but found " + close_tag.Value);

                    var parameters = new List<KeyValuePair<string, string>>();
                    Match match2 = ParamSearcher.Match(match1.Groups[2].Value, 0);
                    for(int j = 0; match2.Success; match2 = ParamSearcher.Match(match1.Groups[2].Value, j)){
                        var name = match2.Groups[1].Value;
                        var value = match2.Groups[2].Value;
                        parameters.Add(new KeyValuePair<string, string>(name, value));
                        j += match2.Length;
                    }

                    var elements = transformers[open_tag.Value].Transform(parameters, match1.Groups[3].Value);
                    block.Inlines.AddRange(elements);
                    i += match1.Length;
                }else{
                    break;
                }
            }

            parsed = result ? block : null;
            return result;
        }
    }
}
