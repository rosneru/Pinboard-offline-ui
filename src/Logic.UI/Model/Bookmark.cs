using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.UI.Model
{
  public partial class Bookmark
  {
    public string HRef { get; set; }
    public string Description { get; set; }
    public string Extended { get; set; }
    public string Meta { get; set; }
    public string Hash { get; set; }
    public string Time { get; set; }
    public string Shared { get; set; }
    public string ToRead { get; set; }
    public string Tags { get; set; }
  }
}
