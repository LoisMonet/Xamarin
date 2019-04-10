using System;
using System.Collections.Generic;
using Xamarin.Forms.Maps;

namespace Fourplaces.Models
{

    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
    }
}
