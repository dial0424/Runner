public class MapTile
{
    public int extend;
    public RenderTexture rt;

    //�ڵ� ����
    public MapTile(bool n, int[] _tiles, int _x, int _y, int _w, int _h, float _ms, float[] _mr, Texture[] _texs, Texture _tex, float _arriveX)
    {
        extend = (int)((float)MainCamera.devWidth * 720 / MainCamera.devHeight - MainCamera.devWidth); //720;
                                                                                                       //w - dw = dw * 720 / dh - dw;
        rt = new RenderTexture(MainCamera.devWidth + extend, 720, 32, RenderTextureFormat.ARGB32);
        
        arriveX = tileX * tileW - _arriveX - extend;
        offMin.x = MainCamera.devWidth + extend - tileX * tileW;

        //�ڵ� ����
    }
    //�ڵ� ����
}