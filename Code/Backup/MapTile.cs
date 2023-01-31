public class MapTile
{
    public MapTile(bool n, int[] _tiles, int _x, int _y, int _w, int _h, float _ms, float[] _mr, Texture[] _texs, Texture _tex, float _arriveX)
    {
        rt = new RenderTexture(MainCamera.devWidth + extend, 720, 32, RenderTextureFormat.ARGB32);

        //관련없는 코드 생략
    }
    
    void update(float dt)
    {
        if (dt == 0)
            return;

        RenderTexture bk = RenderTexture.active;
        RenderTexture.active = rt;                  

        //real paint
        //맵 요소 그리는 코드 생략
        //아이템, 장애물, 타일

        RenderTexture.active = bk;
    }

    public void paint(float dt, float y)
    {
        update(dt);
        iGUI.instance.drawImage(rt, 0, y, MainCamera.resolutionRate, MainCamera.resolutionRate, iGUI.TOP | iGUI.LEFT, 2, 0, iGUI.REVERSE_NONE);
    }
}