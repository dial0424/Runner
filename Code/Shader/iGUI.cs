public class iGUI : MonoBehaviour
{
	public void drawImage(Texture tex, float x, float y, float sx, float sy, int anc, int xyz, float degree, int reverse, 
				float tx = 0f, float ty = 0f, float tw = 1f, float th = 1f)
	{
		//shader와 관련없는 코드 생략
		
		Material m = mat[matIndex];
		m.SetTexture("_MainTex", tex);
		m.SetColor("color", color);
		m.SetFloat("blur", blur);
		m.SetFloat("moz", moz);
		Graphics.DrawTexture(new Rect(-w / 2, -h / 2, w, h), 
							 tex, new Rect(tx, ty, tw, th), 0, 0, 0, 0, m);
		GUI.matrix = matrixPrjection;
	}

	Material[] mat = null;
	int matIndex = 0;
	
	public void setBlendFunc(int index)
	{
		if( mat==null )
		{
			string[] name = new string[] { "std", "add", "blur", "moz" };

			mat = new Material[name.Length];
			for (int i = 0; i < name.Length; i++)
			{
				Shader shader = Shader.Find("Unlit/" + name[i]);
				mat[i] = new Material(shader);
			}
		}	

		matIndex = index;
	}

	float blur = 0;
	public void setBlur(float b)
	{
		blur = b;
	}
	
	float moz = 1;
	public void setMoz(float m)
	{
		if (m < 1)
			m = 1;
		moz = m;
	}
}