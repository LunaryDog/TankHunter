public class Boundary  {
    public float RightBound { get; set; }
    public float LeftBound { get; set; }
    public float UpBound { get; set; }
    public float DownBound { get; set; }

    public Boundary()
    {
        RightBound = 0.0f;
        LeftBound = 0.0f;
        UpBound = 0.0f;
        DownBound = 0.0f;
    }

    public Boundary(float wight,float hight, float offsetX = 0.0f, float offsetY = 0.0f)
    {
        RightBound = 1f * wight/2 + offsetX;
        LeftBound = -1f * wight / 2 + offsetX;
        UpBound = 1f * hight / 2 + offsetY;
        DownBound = -1f * hight / 2 + offsetY;
       
    }
}
