

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Storm.Mvvm;
using Xamarin.Forms;

namespace Fourplaces.Models
{
    public class PlaceItemSummary : NotifierBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image_id")]
        public int ImageId { get; set; }

        public ImageSource SourceImage;
        public ImageSource SOURCEIMAGE
        {

            get
            {

                Task t =GetImageResource();
                return SourceImage;

            }

            set
            {
                SetProperty(ref SourceImage, value);
            }

        }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        public async Task GetImageResource()
        {
            if (SourceImage == null)
            {
                SOURCEIMAGE = await SingletonRestService.RS.GetRequestImage(ImageId);
            }
        }
    }


}