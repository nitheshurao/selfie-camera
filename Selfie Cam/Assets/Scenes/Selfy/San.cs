using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;


public class San : MonoBehaviour
{
    int currentCamIndex = 0;

    WebCamTexture tex;

    public RawImage display;

    public RawImage CapImage;

    public RawImage ArImage;
    //Creteing ibject poolonh 
   
    public Text startStopText;
    public Text imgc;

    //galllery
    int n;
    List<string> imagess = new List<string>();

    public Text debug;
    #region Camere

    //WebCamDevice[] devices = WebCamTexture.devices;
    public void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
            Debug.Log(devices[i].name);

    }


    public void Selfie()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (var device in devices)
        {
            if (device.isFrontFacing)
            {
                tex = new WebCamTexture( Screen.width, Screen.height);
            }
        }
        //for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++)
        //{
        //    if (!WebCamTexture.devices[cameraIndex].isFrontFacing == true)
        //    {
        //        tex = new WebCamTexture(cameraIndex, Screen.width, Screen.height);
        //    }
        //}
        tex.Play();
        display.texture = tex;






    }

    public void back() {
    }
    public void SwapCam_Clicked()
    {

        //Quaternion TestRotation1 = Quaternion.Euler(0, 0, -90);

        //display.transform.rotation = TestRotation1;
        if (WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;

            //Quaternion TestRotation = Quaternion.Euler(0,0,90);

            //display.transform.rotation = TestRotation;

            // if tex is not null:
            // stop the web cam
            // start the web cam

            if (tex != null)
            {
                StopWebCam();
                StartStopCam_Clicked();
                
            }


        }
    }

    public void StartStopCam_Clicked()
    {
        if (tex != null) // Stop the camera
        {
            StopWebCam();
            startStopText.text = "Start Camera";
        }
        else // Start the camera
        {
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            display.texture = tex;

            tex.Play();
            startStopText.text = "Stop Camera";
            CapImage.texture = tex;

        }
    }
    
    private void StopWebCam()
    {
        display.texture = null;
        tex.Stop();
        tex = null;
    }
#endregion

    public void Take()
    { 

     
        TakePhoto(3.0f);
        StartCoroutine(TakePhoto(3.0f));
        Debug.Log("click");


    }

    #region Take Photo
    int j = 1;
    public IEnumerator TakePhoto(float t)  // Start this Coroutine on some button click
    {

        
        Debug.Log("hi");
      


        Texture2D photo = new Texture2D(tex.width, tex.height);
        photo.SetPixels(tex.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        //Debug.Log("done" + p);

        //Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        //ss.Apply();

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(photo, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));

        Debug.Log("Permission result: " + permission);



        //Convet byte to base64
        string base64String = BytoBase64(bytes);
        //StoreImage(base64String, j);

        imagess.Add(base64String);
        imgc.text = "no"+imagess.Count;
        Debug.Log(imagess.Count);
       

        //string base64String = System.Convert.ToBase64String(bytes);
        Debug.Log("done" + base64String);
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes(Application.dataPath + "photo  .png", bytes);
        File.WriteAllText(Application.dataPath + "/Photos.png", base64String);


        //Convet String to byte
        //imgc.text = "Img1 =" + j;
        //j++;


        //byte[] imageBytes = System.Convert.FromBase64String(base64String);
        byte[] imageBytes = Base64toByte(base64String);
        //Display Pic in side reowimage
        Dispalypic(imageBytes);

        //Texture2D text = new Texture2D(2, 2);
        //text.LoadImage(imageBytes);
        //CapImage.texture = text;
        yield return new WaitForEndOfFrame();
    }
    #region Arrayof image
    private void StoreImage(string img)
    {
        //public string[] imagess = new string[n];
    
      

    }

    #endregion


    #endregion
    #region Display Pic in side reowimage
    private void Dispalypic(byte[] image)
    {
        Texture2D text = new Texture2D(2, 2);
        text.LoadImage(image);
        CapImage.texture = text;
        Instantiate(CapImage);
        ArImage.texture = text;



    }

    #endregion


    #region Convert Byte toBase64 

    // 
    public static string BytoBase64(byte[] bytes) {
        string base64String = System.Convert.ToBase64String(bytes);
        return base64String;

    }
    #endregion
    #region Convert Base64 to Byte
    public static byte[] Base64toByte(string base64String)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64String);
        return imageBytes;
    }
    #endregion

 

   public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //Texture2D text = new Texture2D(2, 2);
                //text.LoadImage(image);
                CapImage.texture = texture;
                ArImage.texture = texture;
                //Instantiate(CapImage);
                // Assign texture to a temporary quad and destroy it after 5 seconds
             GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                Material material = quad.GetComponent<Renderer>().material;
                if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                material.mainTexture = texture;
                //Dispalypic(texture);
                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
        });
    }
    }