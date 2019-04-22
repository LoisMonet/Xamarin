using System;

using Xamarin.Forms;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TD.Api.Dtos;
using System.Text;
using System.Net.Http.Headers;
using System.Diagnostics;
using Common.Api.Dtos;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using Fourplaces.Models.Exceptions;
using System.Net;
using MonkeyCache.SQLite;

namespace Fourplaces.Models
{
    public class RestService
    {

        HttpClient client;
        public String url= "https://td-api.julienmialon.com/";
        public RestService()
        {

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

        }


        public async Task<List<PlaceItemSummary>> RefreshDataAsync()
        {
            var uri = new Uri(string.Format(url + "places", string.Empty));

            CheckInternetConnectionForCache(uri.ToString());

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));
                Response < List<PlaceItemSummary> > r = JsonConvert.DeserializeObject< Response<List<PlaceItemSummary>>>(content);


                return r.Data;


            }

            return null;



        }

        public async Task<PlaceItem> PlaceItemDataAsync(int id)
        {

            var uri = new Uri(string.Format(url + "places/" + id, string.Empty));

            CheckInternetConnectionForCache(uri.ToString());

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));

                Response<PlaceItem> r = JsonConvert.DeserializeObject<Response<PlaceItem>>(content);
      
                return r.Data;
            }

            return null;

        }

        public async Task SendCommentDataAsync(int id,String comment)
        {
            CheckInternetConnection();

            await isConnected();

            if (string.IsNullOrEmpty(comment))
            {

                throw new Exception("Le champ commentaire est vide");
            }

            var uri = new Uri(string.Format(url + "places/" + id+"/comments", string.Empty));

            CreateCommentRequest ccr = new CreateCommentRequest();
            ccr.Text = comment;
            var jsonRequest = JsonConvert.SerializeObject(ccr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Response r = JsonConvert.DeserializeObject<Response>(result);


            }
            else
            {
                Debugger.Break();

            }




        }



        public async Task<bool> SendPlaceDataAsync(String nom, String description,String lattitude,String longitude, byte[] imageData,LoginResult lr)
        {
            
            CheckInternetConnection();

            await isConnected();

            if (imageData == null)
            {
                throw new AddPlaceE("Vous n'avez pas ajouté d'image");
            }

            if (nom == ""|| description == "" || string.IsNullOrEmpty(lattitude)|| string.IsNullOrEmpty(longitude))
            {
                throw new AddPlaceE(nom, description, lattitude, longitude);
            }


            var uri = new Uri(string.Format(url + "places/", string.Empty));
            double lattitudeD;
            double longitudeD;

            try
            {
                lattitudeD = double.Parse(lattitude, System.Globalization.CultureInfo.InvariantCulture);
                longitudeD = double.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture);

                if (lattitudeD > 90 || lattitudeD < -90 || longitudeD > 180 || longitudeD < -180 || lattitudeD.Equals(0) || longitudeD.Equals(0))
                {
                    throw new AddPlaceE(lattitudeD, longitudeD);
                }

            }
            catch (System.FormatException)
            {
                throw new AddPlaceE("mauvais format de la latitude ou de la longitude(ex 5 ou 5.2)");
            }



            CreatePlaceRequest cpr = new CreatePlaceRequest();
            cpr.Title = nom;
            cpr.Description = description;

            ImageItem iItem= await UploadImage(imageData);
            cpr.ImageId =iItem.Id ;

            cpr.Latitude = lattitudeD;
            cpr.Longitude = longitudeD;


            var jsonRequest = JsonConvert.SerializeObject(cpr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lr.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Response r  = JsonConvert.DeserializeObject<Response>(result);
         

                return true;


            }
            else
            {
                Debugger.Break();

            }

            return false;




        }

        public async Task<byte[]> SendPicture(bool camera)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                //Supply media options for saving our photo after it's taken.
                var pictureMediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg",
                    PhotoSize = PhotoSize.Small

                };

                var galleryMediaOptions = new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small

                };

                // Take a photo of the business receipt.
                MediaFile file;
                if (camera)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(pictureMediaOptions);

                }
                else{
                    file = await CrossMedia.Current.PickPhotoAsync(galleryMediaOptions);

                }

                if (file != null)
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    byte[] imageData = GetImageStreamAsBytes(stream);
                    return imageData;
                }

                return null;


            }

            return null;
        }

        public byte[] GetImageStreamAsBytes(Stream input) 
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task<ImageItem> UploadImage(byte[] imageData)
        {
            CheckInternetConnection();


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SingletonLoginResult.LR.AccessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

          

            if (response.IsSuccessStatusCode)
            {
                Response<ImageItem> r = JsonConvert.DeserializeObject<Response<ImageItem>>(result);
             

                return r.Data;
            }
            else
            {
                Debugger.Break();
                return null;
            }
        }

        public async Task<LoginResult> RegisterDataAsync(String email, String fname, String lname, String mdp)
        {
            CheckInternetConnection();

            var uri = new Uri(string.Format(url + "auth/register", string.Empty));


            if (email == "" || mdp == "" || fname=="" || mdp=="")
            {
                throw new RegisterE(email, mdp, fname, lname);
            }



            RegisterRequest rr = new RegisterRequest();
            rr.Email = email;
            rr.FirstName = fname;
            rr.LastName = lname;
            rr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(rr);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));
                Barrel.Current.Empty(key: "Account"); 

                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
            

                if (r.IsSuccess) { 

                    return r.Data;
                }
                
            }
            throw new RegisterE("email déjà pris");

        }


        public async Task RefreshAsync() 
        {
            CheckInternetConnection();


            var uri = new Uri(string.Format(url + "auth/refresh", string.Empty));




            RefreshRequest rr = new RefreshRequest();
            rr.RefreshToken = SingletonLoginResult.LR.RefreshToken;
            var jsonRequest = JsonConvert.SerializeObject(rr);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
           

                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));

                    SingletonLoginResult.destroyLR();
                    SingletonLoginResult.LR = r.Data;
                }

            }


        }

        public async Task<LoginResult> ConnexionDataAsync(String email, String mdp)
        {
            CheckInternetConnection();

            var uri = new Uri(string.Format(url + "auth/login", string.Empty));


            if (email=="" || mdp == "")
            {
                throw new ConnexionE(email, mdp);
            }





            LoginRequest lr = new LoginRequest();
            lr.Email = email;

            lr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(lr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");



            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));
                Barrel.Current.Empty(key: "Account");

                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
    
                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }

            throw new ConnexionE("mauvais email ou mot de passe");

        }

        public async Task<UserItem> UserDataAsync()
        {
            CheckInternetConnection();

            await isConnected();

            var uri = new Uri(string.Format(url + "me", string.Empty));



            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
      
                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "Account", data: result, expireIn: TimeSpan.FromDays(1));

                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<UserItem> EditCountAsync(String FName, String LName,int? imageId,byte[] imageData)
        {
            CheckInternetConnection();

            await isConnected();

            if (FName == "" || LName == "")
            {
                throw new EditCompteE(FName, LName);
            }

            var uri = new Uri(string.Format(url + "me", string.Empty));


            UpdateProfileRequest upr = new UpdateProfileRequest();
            upr.FirstName = FName;
            upr.LastName = LName;

            if (imageData == null) 
            {
                upr.ImageId = imageId;
            }
            else
            {
                ImageItem iItem = await UploadImage(imageData);
                upr.ImageId = iItem.Id;
            }




            var jsonRequest = JsonConvert.SerializeObject(upr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

        

            if (response.IsSuccessStatusCode)
            {
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
              
                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "Account", data: result, expireIn: TimeSpan.FromDays(1));
                    return r.Data;
                }




            }
            else
            {
                Debugger.Break();

            }

            return null;

        }

        public async Task<UserItem> EditPWAsync(String oldPW,String newPW)
        {
            CheckInternetConnection();

            await isConnected();

            if (oldPW==null || newPW==null || oldPW=="" ||newPW=="") //SEE AGAIN
            {
          
                throw new PwdCompteE(oldPW, newPW);
            }


            var uri = new Uri(string.Format(url + "me/password", string.Empty));



            UpdatePasswordRequest upr = new UpdatePasswordRequest();
            upr.OldPassword=oldPW;
            upr.NewPassword=newPW;
            var jsonRequest = JsonConvert.SerializeObject(upr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

      

            if (response.IsSuccessStatusCode)
            {
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
             

                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "Account", data: result, expireIn: TimeSpan.FromDays(1));
                    return r.Data;
                }




            }

            throw new PwdCompteE("L'ancien mot de passe n'est pas correct");

        }

        public async Task<ImageSource> GetRequestImage(int id)
        {
            var uri = new Uri(string.Format(url + "images/" + id, string.Empty));
            CheckInternetConnectionForCache(uri.ToString());

            if (Barrel.Current.Exists(uri.ToString())) //cache exist already
            {
                var content = Barrel.Current.Get<byte[]>(key: uri.ToString());

                ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));
                return ims;

            }
            else //download image
            {
              
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);


                if (response.IsSuccessStatusCode)
                {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();



                    ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));


                    Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));


                    return ims;


                }

                return "placeDef.png";
            }

        }

        public async Task<ImageSource> GetRequestImageProfil(int? id)
        {
            var uri = new Uri(string.Format(url + "images/" + id, string.Empty));
            CheckInternetConnectionForCache(uri.ToString());

            if (id == null)
            {
                return "profilDef.png";
            }
            else //download image
            {
               
                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);


                if (response.IsSuccessStatusCode)
                {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();

                    ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));


                    Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));


                    return ims;


                }

                return "profilDef.png";
            }

        }

        bool firstCo=true;

        public async Task  isConnected()
        {
            if(Barrel.Current.Exists("LoginResult") && firstCo)
            {
                await RefreshAsync();
            }

            firstCo = false;

            if (SingletonLoginResult.LR == null)
            {

                throw new Exception("vous n'êtes pas connecté à un compte");


            }
           
            if (SingletonLoginResult.LR.IsExpired())
            {
                await RefreshAsync();
            }



        }

        public void CheckInternetConnection()
        {
            string CheckUrl = url;

            try
            {
                HttpWebRequest testRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                testRequest.Timeout = 1000;

                WebResponse testResponse = testRequest.GetResponse();

                testResponse.Close();

            }
            catch (WebException)
            {
                throw new NoConnectE("vous n'êtes pas connecté à internet");
            }
        }

        public void CheckInternetConnectionForCache(String urlSave)
        {
            string CheckUrl = url;

            try
            {
                HttpWebRequest testRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                testRequest.Timeout = 1000;

                WebResponse testResponse = testRequest.GetResponse();

                testResponse.Close();

            }
            catch (WebException)
            {
                throw new NoConnectE(urlSave, "vous n'êtes pas connecté à internet");
            }
        }


        public T CacheData<T>(String k)
        {
            if (!Barrel.Current.IsExpired(key: k))
            {
                var content= Barrel.Current.Get<String>(key: k);
                Response<T> r = JsonConvert.DeserializeObject<Response<T>>(content);
                return r.Data;
            }
            return default(T);
        }

        public ImageSource CacheImage(String k)
        {
            if (!Barrel.Current.IsExpired(key: k))
            {
                var content = Barrel.Current.Get<byte[]>(key: k);

                ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));
                return ims;
            }

            return null;
        }

    }


}

