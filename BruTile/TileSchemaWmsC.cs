// Copyright 2008 - Paul den Dulk (Geodan)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Tiling
{
  public class TileSchemaWmsC : TileSchema
  {
    #region Fields

    private List<string> layers = new List<string>();
    private List<string> styles = new List<string>();
    private const bool useOnlyFirstResolution = false;
 
   #endregion

    #region Properties

    private IList<string> Layers
    {
      get { return layers; }
    }

    public IList<string> Styles
    {
      get { return styles; }
    }

    public override string Additions
    {
      get
      {
        StringBuilder result = new StringBuilder();
        result.AppendFormat("&LAYERS={0}", ToCommaSeparatedValues(Layers));
        result.AppendFormat("&STYLES={0}", ToCommaSeparatedValues(Styles));
        return result.ToString();
      }
    }

    #endregion

    private static string ToCommaSeparatedValues(IList<string> items)
    {
      StringBuilder result = new StringBuilder();
      foreach (string str in items)
      {
        result.AppendFormat(CultureInfo.InvariantCulture, ",{0}", str);
      }
      if (result.Length > 0) result.Remove(0, 1);
      return result.ToString();
    }

    public override void Validate()
    {
      if (layers.Count == 0)
      {
        throw new ValidationException(String.Format(
          CultureInfo.InvariantCulture, "No Layers were set for the TileSchema"));
      }
      //Note: An empty Style list is allowed so dont check for Style
      base.Validate();
    }

    public override string ToString()
    {
      return this.Name;
    }

  }
}
