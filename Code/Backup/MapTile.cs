public class MapTile
{
    public MapTile(bool n, int[] _tiles, int _x, int _y, int _w, int _h, float _ms, float[] _mr, Texture[] _texs, Texture _tex, float _arriveX)
    {
        rt = new RenderTexture(MainCamera.devWidth + extend, 720, 32, RenderTextureFormat.ARGB32);

        //���þ��� �ڵ� ����
    }
    
    void update(float dt)
    {
        if (dt == 0)
            return;

        RenderTexture bk = RenderTexture.active;
        RenderTexture.active = rt;                  

        //real paint
        //�� ��� �׸��� �ڵ� ����
        //������, ��ֹ�, Ÿ��

        RenderTexture.active = bk;
    }

    public void paint(float dt, float y)
    {
        update(dt);
        iGUI.instance.drawImage(rt, 0, y, MainCamera.resolutionRate, MainCamera.resolutionRate, iGUI.TOP | iGUI.LEFT, 2, 0, iGUI.REVERSE_NONE);
    }
}