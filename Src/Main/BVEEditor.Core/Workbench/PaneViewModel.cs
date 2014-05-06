/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/07/10
 * Time: 12:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Media;
using Caliburn.Micro;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// Represents a pane view model. It contains all the stuff that's needed for all pane items.
	/// </summary>
	public abstract class PaneViewModel : Screen
	{
		public ImageSource Icon{
            get; 
            protected set;
        }

        string title;
        public string Title{
            get{return title;}
            set{
                if(title != value){
                    title = value;
                    NotifyOfPropertyChange(() => Title);
                }
            }
        }

        string content_id;
        public string ContentId{
            get{return content_id;}
            set{
                if(content_id != value){
                    content_id = value;
                    NotifyOfPropertyChange(() => ContentId);
                }
            }
        }

        bool is_visible;
        public bool IsVisible{
            get{return is_visible;}
            set{
                if(is_visible != value){
                    is_visible = value;
                    NotifyOfPropertyChange(() => IsVisible);
                }
            }
        }
	}
}
