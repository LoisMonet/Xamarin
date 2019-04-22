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
            
            Console.WriteLine("RestService");

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

        }


        public async Task<List<PlaceItemSummary>> RefreshDataAsync()
        {
            var uri = new Uri(string.Format(url + "places", string.Empty));

            CheckInternetConnectionForCache(uri.ToString());
            //CheckInternetConnection();

            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Dev_Refresh:" + content);
                Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));
                Response < List<PlaceItemSummary> > r = JsonConvert.DeserializeObject< Response<List<PlaceItemSummary>>>(content);


                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);
                return r.Data;


            }

            return null;



        }

        public async Task<PlaceItem> PlaceItemDataAsync(int id)
        {
            //CheckInternetConnection();

            var uri = new Uri(string.Format(url + "places/" + id, string.Empty));

            CheckInternetConnectionForCache(uri.ToString());


            Console.WriteLine("Dev_BefResp:");

            var response = await client.GetAsync(uri);

            Console.WriteLine("Dev_statusCode:" + response.IsSuccessStatusCode);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                Barrel.Current.Add(key: uri.ToString(), data: content, expireIn: TimeSpan.FromDays(1));

                Response<PlaceItem> r = JsonConvert.DeserializeObject<Response<PlaceItem>>(content);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                return r.Data;
            }

            return null;

        }

        public async Task SendCommentDataAsync(int id,String comment)
        {
            CheckInternetConnection();

            await isConnected();

            if (string.IsNullOrEmpty(comment)) //SEE AGAIN
            {
                //Console.WriteLine("Exception:" + oldPW + "|" + newPW);

                //throw new AuthenticationException("Le champ commentaire est vide"); //SEE AGAIN THIS METHOD LATER
                throw new Exception("Le champ commentaire est vide");
            }

            var uri = new Uri(string.Format(url + "places/" + id+"/comments", string.Empty));

            Console.WriteLine("Dev_SCDBefResp:");

            CreateCommentRequest ccr = new CreateCommentRequest();
            ccr.Text = comment;
            var jsonRequest = JsonConvert.SerializeObject(ccr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Dev_token:"+SingletonLoginResult.LR.AccessToken);
            Console.WriteLine("Dev_SCDResponse:" + result);
            Console.WriteLine("Dev_SCDStatusCode:" + response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Response r = JsonConvert.DeserializeObject<Response>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                //return true;
                /*if (d.is_success)
                {

                    //return d.data;
                }*/

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
                //throw new AuthenticationException("Vous n'avez pas ajouté d'image");
                throw new AddPlaceE("Vous n'avez pas ajouté d'image");
            }

            if (nom == ""|| description == "" || string.IsNullOrEmpty(lattitude)|| string.IsNullOrEmpty(longitude))
            {
                //throw new AuthenticationException(nom,description, lattitude, longitude,0); //SEE AGAIN THIS METHOD LATER
                throw new AddPlaceE(nom, description, lattitude, longitude);
            }


            var uri = new Uri(string.Format(url + "places/", string.Empty));
            double lattitudeD;
            double longitudeD;

            Console.WriteLine("Dev_SendPlaceData:lattitude:" + lattitude + "|longitude:" + longitude);
            try
            {
                lattitudeD = double.Parse(lattitude, System.Globalization.CultureInfo.InvariantCulture);
                longitudeD = double.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture);

                if (lattitudeD > 90 || lattitudeD < -90 || longitudeD > 180 || longitudeD < -180 || lattitudeD.Equals(0) || longitudeD.Equals(0))
                {
                    //throw new AuthenticationException(lattitudeD, longitudeD);
                    throw new AddPlaceE(lattitudeD, longitudeD);
                }

            }
            catch (System.FormatException)
            {
                //throw new AuthenticationException("mauvais format de la latitude ou de la longitude(ex 5 ou 5.2)");
                throw new AddPlaceE("mauvais format de la latitude ou de la longitude(ex 5 ou 5.2)");
            }



            CreatePlaceRequest cpr = new CreatePlaceRequest();
            cpr.Title = nom;
            cpr.Description = description;

            ImageItem iItem= await UploadImage(imageData);
            cpr.ImageId =iItem.Id ;

            //cpr.ImageId = 37;
            cpr.Latitude = lattitudeD;
            cpr.Longitude = longitudeD;

            Console.WriteLine("Dev_SendPlaceData:lattitudeD:" + lattitudeD + "|longitudeD:" + longitudeD+"|imageId:"+cpr.ImageId);

            var jsonRequest = JsonConvert.SerializeObject(cpr);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lr.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_RDResponse:" + result);
            Console.WriteLine("Dev_RDStatusCode:" + response.StatusCode); 
            if (response.IsSuccessStatusCode)
            {
                Response r  = JsonConvert.DeserializeObject<Response>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                return true;
                /*if (d.is_success)
                {

                    //return d.data;
                }*/

            }
            else
            {
                Debugger.Break();

            }

            return false;
            //return null;



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
                

                //if (file == null) MODIFY CODE HERE ELSE NULL IF QUIT PICTURE
                //{
                //    file = "profileDef.png";
                //}
                Console.WriteLine("picture:");
                Console.WriteLine("picture:" + file);
                if (file != null)
                {
                    var stream = file.GetStream();
                    file.Dispose();
                    byte[] imageData = GetImageStreamAsBytes(stream);
                    return imageData;
                }

                return null;




                //return file;
            }

            return null;
        }

        public byte[] GetImageStreamAsBytes(Stream input) //Put in private later
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

            //Stream mf=await SendPicture(camera);
            //byte[] imageData = GetImageStreamAsBytes(mf);
            //byte[] imageData = await client.GetByteArrayAsync("https://bnetcmsus-a.akamaihd.net/cms/blog_header/x6/X6KQ96B3LHMY1551140875276.jpg");
            //byte[] imageData = mf.GetStream;

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

            Console.WriteLine("Dev_RDResponse:" + result);
            Console.WriteLine("Dev_RDStatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Image uploaded!");
                Response<ImageItem> r = JsonConvert.DeserializeObject<Response<ImageItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

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

            //CheckInternetConnectionForCache(uri.ToString());

            if (email == "" || mdp == "" || fname=="" || mdp=="")
            {
                //throw new AuthenticationException(email, mdp,fname,lname);
                throw new RegisterE(email, mdp, fname, lname);
            }



            Console.WriteLine("Dev_RegisterData:");



            RegisterRequest rr = new RegisterRequest();
            rr.Email = email;
            rr.FirstName = fname;
            rr.LastName = lname;
            rr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(rr);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));
                Barrel.Current.Empty(key: "Account"); //Test

                Console.WriteLine("Dev_RDResponse:" + result);
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess) { 

                    return r.Data;
                }
                
            }
            //throw new AuthenticationException("email déjà pris");
            throw new RegisterE("email déjà pris");

        }


        public async Task RefreshAsync() //need to be call everytime
        {
            CheckInternetConnection();


            var uri = new Uri(string.Format(url + "auth/refresh", string.Empty));

            Console.WriteLine("Dev_Refresh:");



            RefreshRequest rr = new RefreshRequest();
            rr.RefreshToken = SingletonLoginResult.LR.RefreshToken;
            var jsonRequest = JsonConvert.SerializeObject(rr);


            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_RDResponse:" + result);
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));

                    SingletonLoginResult.destroyLR();
                    SingletonLoginResult.LR = r.Data;
                    //Console.WriteLine("DevRefffresh:"+ SingletonLoginResult.LR.AccessToken);
                }

            }


        }

        public async Task<LoginResult> ConnexionDataAsync(String email, String mdp)
        {
            CheckInternetConnection();

            var uri = new Uri(string.Format(url + "auth/login", string.Empty));

            //CheckInternetConnectionForCache(uri.ToString());

            if (email=="" || mdp == "")
            {
                //throw new AuthenticationException(email,mdp);
                throw new ConnexionE(email, mdp);
            }



            Console.WriteLine("Dev_ConnexionData:");


            LoginRequest lr = new LoginRequest();
            lr.Email = email;

            lr.Password = mdp;
            var jsonRequest = JsonConvert.SerializeObject(lr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "__access__token__");
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                Barrel.Current.Add(key: "LoginResult", data: result, expireIn: TimeSpan.FromDays(1));
                Barrel.Current.Empty(key: "Account");

                Console.WriteLine("Dev_CDResponse:" + result);
                Response<LoginResult> r = JsonConvert.DeserializeObject<Response<LoginResult>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {

                    return r.Data;
                }




            }

            //throw new AuthenticationException("mauvais email ou mot de passe");
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
                Console.WriteLine("Dev_UDResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

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
                //throw new AuthenticationException(FName, LName, 0); //SEE AGAIN THIS METHOD LATER
                throw new EditCompteE(FName, LName);
            }

            var uri = new Uri(string.Format(url + "me", string.Empty));

            Console.WriteLine("Dev_ECA:");


            UpdateProfileRequest upr = new UpdateProfileRequest();
            upr.FirstName = FName;
            upr.LastName = LName;

            if (imageData == null) //ne pas mettre a jour l'image de profil
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
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_ECAesponse:" + result);
            Console.WriteLine("Dev_ECAtatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_ECAResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

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
                //Console.WriteLine("Exception:" + oldPW + "|" + newPW);
                //throw new AuthenticationException(oldPW, newPW, true); //SEE AGAIN THIS METHOD LATER
                throw new PwdCompteE(oldPW, newPW);
            }


            var uri = new Uri(string.Format(url + "me/password", string.Empty));



            UpdatePasswordRequest upr = new UpdatePasswordRequest();
            upr.OldPassword=oldPW;
            upr.NewPassword=newPW;
            var jsonRequest = JsonConvert.SerializeObject(upr);



            var content = new StringContent(jsonRequest, Encoding.UTF8, "text/json");
            //var response = client.PostAsync(uri, content).Result;



            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri);
            request.Headers.Authorization = new AuthenticationHeaderValue(SingletonLoginResult.LR.TokenType, SingletonLoginResult.LR.AccessToken);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dev_ECAesponse:" + result);
            Console.WriteLine("Dev_ECAtatusCode:" + response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Dev_ECAResponse:" + result);
                Response<UserItem> r = JsonConvert.DeserializeObject<Response<UserItem>>(result);
                Console.WriteLine("Dev_is_sucess:" + r.IsSuccess);
                Console.WriteLine("Dev_error_code:" + r.ErrorCode);
                Console.WriteLine("Dev_error_message:" + r.ErrorMessage);

                if (r.IsSuccess)
                {
                    Barrel.Current.Add(key: "Account", data: result, expireIn: TimeSpan.FromDays(1));
                    return r.Data;
                }




            }

            //throw new AuthenticationException("L'ancien mot de passe n'est pas correct");
            throw new PwdCompteE("L'ancien mot de passe n'est pas correct");

        }

        public async Task<ImageSource> GetRequestImage(int id)
        {
            var uri = new Uri(string.Format(url + "images/" + id, string.Empty));
            //CheckInternetConnection();
            CheckInternetConnectionForCache(uri.ToString());

            if (Barrel.Current.Exists(uri.ToString())) //cache exist already
            {
                Console.WriteLine("Dev_Exists:"+uri.ToString());
                var content = Barrel.Current.Get<byte[]>(key: uri.ToString());

                ImageSource ims = ImageSource.FromStream(() => new MemoryStream(content));
                return ims;

            }
            else //download image
            {
                Console.WriteLine("Dev_GetRequestImage:" + id);




                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                Console.WriteLine("Dev_GetRequestImageNothing");
                Console.WriteLine("Dev_GetRequestImageGetURI:" + response.StatusCode);
                Console.WriteLine("Dev_GetRequestImageStatus:" + response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();

                    Console.WriteLine("Dev_ImageAll:");

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
            //CheckInternetConnection();
            CheckInternetConnectionForCache(uri.ToString());

            if (id == null)
            {
                return "profilDef.png";
            }
            else //download image
            {
                Console.WriteLine("Dev_GetRequestImageProfil:" + id);




                var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                Console.WriteLine("Dev_GetRequestImageProfilNothing");
                Console.WriteLine("Dev_GetRequestImageProfilGetURI:" + response.StatusCode);
                Console.WriteLine("Dev_GetRequestImageProfilStatus:" + response.IsSuccessStatusCode);

                if (response.IsSuccessStatusCode)
                {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();

                    Console.WriteLine("Dev_ImageAll:");

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
                Console.WriteLine("Dev_refreshForCache");
                await RefreshAsync();
            }

            firstCo = false;

            if (SingletonLoginResult.LR == null)
            {

                //throw new AuthenticationException("vous n'êtes pas connecté");
                throw new Exception("vous n'êtes pas connecté à un compte");


            }
            //Console.WriteLine("Dev_OldconnectedToken:" + SingletonLoginResult.LR.AccessToken);
            //await RefreshAsync();
            Console.WriteLine("Dev_connectedToken:" + SingletonLoginResult.LR.AccessToken);
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

                // Console.WriteLine ("...connection established..." + iNetRequest.ToString ());
                testResponse.Close();

            }
            catch (WebException ex)
            {
                Console.WriteLine("Dev_checkCo:"+ex.Message); 

                //throw new AuthenticationException("vous n'êtes pas connecté à internet");
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

                // Console.WriteLine ("...connection established..." + iNetRequest.ToString ());
                testResponse.Close();

            }
            catch (WebException ex)
            {
                Console.WriteLine("Dev_checkCo:" + ex.Message);

                //throw new AuthenticationException(urlSave,0);
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

