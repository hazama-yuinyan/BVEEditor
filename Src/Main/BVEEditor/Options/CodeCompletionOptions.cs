using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Options
{
    /// <summary>
    /// Just a convenient class for code-completion-related options.
    /// </summary>
    public class CodeCompletionOptions : PropertyChangedBase
    {
        static CodeCompletionOptions instance;

        public static CodeCompletionOptions Instance{
            get{
                if(instance == null)
                    instance = IoC.Get<IPropertyService>().Get("CodeCompletionOptions", new CodeCompletionOptions());

                return instance;
            }
        }

        bool enable_code_completion = true;

        /// <summary>
        /// Global option to turn all code-completion-related features off.
        /// </summary>
        public bool EnableCodeCompletion{
            get{
                return enable_code_completion;
            }

            set{
                if(enable_code_completion != value){
                    enable_code_completion = value;
                    NotifyOfPropertyChange(() => EnableCodeCompletion);
                }
            }
        }

        bool data_usage_cache_enabled = true;

        public bool DataUsageCacheEnabled{
            get{
                return data_usage_cache_enabled;
            }

            set{
                if(data_usage_cache_enabled != value){
                    data_usage_cache_enabled = value;
                    NotifyOfPropertyChange(() => DataUsageCacheEnabled);
                }
            }
        }

        int data_usage_cache_item_count = 500;

        public int DataUsageCacheItemCount{
            get{
                return data_usage_cache_item_count;
            }

            set{
                if(data_usage_cache_item_count != value){
                    data_usage_cache_item_count = value;
                    NotifyOfPropertyChange(() => DataUsageCacheItemCount);
                }
            }
        }

        bool tooltips_enabled = false;

        public bool TooltipsEnabled{
            get{
                return tooltips_enabled;
            }

            set{
                if(tooltips_enabled != value){
                    tooltips_enabled = value;
                    NotifyOfPropertyChange(() => TooltipsEnabled);
                }
            }
        }

        bool tooltips_only_when_debugging = false;

        public bool TooltipsOnlyWhenDebugging{
            get{
                return tooltips_only_when_debugging;
            }

            set{
                if(tooltips_only_when_debugging != value){
                    tooltips_only_when_debugging = value;
                    NotifyOfPropertyChange(() => TooltipsOnlyWhenDebugging);
                }
            }
        }

        bool keyword_completion_enabled = true;

        public bool KeywordCompletionEnabled{
            get{
                return keyword_completion_enabled;
            }

            set{
                if(keyword_completion_enabled != value){
                    keyword_completion_enabled = value;
                    NotifyOfPropertyChange(() => KeywordCompletionEnabled);
                }
            }
        }

        bool complete_when_typing = true;

        public bool CompleteWhenTyping{
            get{
                return complete_when_typing;
            }
            
            set{
                if(complete_when_typing != value){
                    complete_when_typing = value;
                    NotifyOfPropertyChange(() => CompleteWhenTyping);
                }
            }
        }

        bool insight_enabled = false;

        public bool InsightEnabled{
            get{
                return insight_enabled;
            }

            set{
                if(insight_enabled != value){
                    insight_enabled = value;
                    NotifyOfPropertyChange(() => InsightEnabled);
                }
            }
        }

        bool insight_referesh_on_comma = true;

        public bool InsightRefreshOnComma{
            get{
                return insight_referesh_on_comma;
            }

            set{
                if(insight_referesh_on_comma != value){
                    insight_referesh_on_comma = value;
                    NotifyOfPropertyChange(() => InsightRefreshOnComma);
                }
            }
        }
    }
}
