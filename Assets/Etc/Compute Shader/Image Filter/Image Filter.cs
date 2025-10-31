using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageFilter : MonoBehaviour
{
    private const string GRAY_SCALE = "GrayScale";
    private const string INVERT = "Invert";
    private const string SEPIA = "Sepia";
    private const string ADJUST_COLOR = "AdjustColor";
    private const string BOX_BLUR = "BoxBlur";
    private const string SHARPEN = "Sharpen";
    private const string POSTERIZE = "Posterize";

    private const string SOURCE_TEXTURE = "_SourceTexture";
    private const string RESULT_TEXTURE = "_ResultTexture";
    private const string BRIGHTNESS = "_Brightness";
    private const string CONTRAST = "_Contrast";
    private const string SATURATION = "_Saturation";
    private const string TEXTURE_WIDTH = "_TextureWidth";
    private const string TEXTURE_HEIGHT = "_TextureHeight";
    private const string POSTERIZE_STEPS = "_PosterizeSteps";

    public ComputeShader imageFilterCompute;
    public Texture sourceTexture;

    [Space, Header("UI-Image")]
    public RawImage displaySourceImage;
    public RawImage displayResultImage;

    [Header("UI-Button")]
    public Button grayScaleButton;
    public Button invertButton;
    public Button sepiaButton;
    public Button boxBlurButton;
    public Button sharpenButton;
    public Button resetButton;

    [Header("UI-Slider")]
    public TMP_Text brightnessText;
    public Slider brightnessSlider;
    public TMP_Text contrastText;
    public Slider contrastSlider;
    public TMP_Text saturationText;
    public Slider saturationSlider;
    public TMP_Text posterizeText;
    public Slider posterizeSlider;


    // Compute shader
    private RenderTexture m_resultRenderTexture;
    private int m_kernalId;

    private void Awake()
    {
        grayScaleButton.onClick.AddListener(GrayScale);
        invertButton.onClick.AddListener(Invert);
        sepiaButton.onClick.AddListener(Sepia);
        boxBlurButton.onClick.AddListener(BoxBlur);
        sharpenButton.onClick.AddListener(Sharpen);

        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);
        contrastSlider.onValueChanged.AddListener(AdjustContrast);
        saturationSlider.onValueChanged.AddListener(AdjustSaturation);
        posterizeSlider.onValueChanged.AddListener(Posterize);

        resetButton.onClick.AddListener(ResetColor);

        displaySourceImage.texture = sourceTexture;

        // Init ResultRenderTexture
        m_resultRenderTexture = new RenderTexture(sourceTexture.width, sourceTexture.height, 0, RenderTextureFormat.ARGB32);
        m_resultRenderTexture.enableRandomWrite = true;
        m_resultRenderTexture.Create();
    }

    private void Start()
    {
        imageFilterCompute.SetInt(TEXTURE_WIDTH, sourceTexture.width);
        imageFilterCompute.SetInt(TEXTURE_HEIGHT, sourceTexture.height);

        imageFilterCompute.SetFloat(BRIGHTNESS, brightnessSlider.value);
        imageFilterCompute.SetFloat(CONTRAST, contrastSlider.value);
        imageFilterCompute.SetFloat(SATURATION, saturationSlider.value);
        DispathKernel(ADJUST_COLOR);
    }

    private void OnDestroy()
    {
        m_resultRenderTexture?.Release();
    }

    private void DispathKernel(string kernelName)
    {
        m_kernalId = imageFilterCompute.FindKernel(kernelName);
        imageFilterCompute.SetTexture(m_kernalId, SOURCE_TEXTURE, sourceTexture);
        imageFilterCompute.SetTexture(m_kernalId, RESULT_TEXTURE, m_resultRenderTexture);

        int threadGroupX = Mathf.CeilToInt(sourceTexture.width / 8.0f);
        int threadGroupY = Mathf.CeilToInt(sourceTexture.height / 8.0f);

        imageFilterCompute.Dispatch(m_kernalId, threadGroupX, threadGroupY, 1);

        displayResultImage.texture = m_resultRenderTexture;
    }

    private void GrayScale()
    {
        DispathKernel(GRAY_SCALE);
    }

    private void Invert()
    {
        DispathKernel(INVERT);
    }

    private void Sepia()
    {
        DispathKernel(SEPIA);
    }

    private void AdjustBrightness(float value)
    {
        brightnessText.text = $"[밝기] [{value.ToString("F2")}]";

        imageFilterCompute.SetFloat(BRIGHTNESS, value);
        DispathKernel(ADJUST_COLOR);
    }

    private void AdjustContrast(float value)
    {
        contrastText.text = $"[대비] [{value.ToString("F2")}]";

        imageFilterCompute.SetFloat(CONTRAST, value);
        DispathKernel(ADJUST_COLOR);
    }

    private void AdjustSaturation(float value)
    {
        saturationText.text = $"[채도] [{value.ToString("F2")}]";

        imageFilterCompute.SetFloat(SATURATION, value);
        DispathKernel(ADJUST_COLOR);
    }

    private void BoxBlur()
    {
        DispathKernel(BOX_BLUR);
    }

    private void Sharpen()
    {
        DispathKernel(SHARPEN);
    }

    private void Posterize(float value)
    {
        posterizeText.text = $"[Posterize] [{value.ToString("F2")}]";

        imageFilterCompute.SetFloat(POSTERIZE_STEPS, value);
        DispathKernel(POSTERIZE);
    }

    private void ResetColor()
    {
        brightnessSlider.value = 1.0f;
        contrastSlider.value = 1.0f;
        saturationSlider.value = 1.0f;

        imageFilterCompute.SetFloat(BRIGHTNESS, brightnessSlider.value);
        imageFilterCompute.SetFloat(CONTRAST, contrastSlider.value);
        imageFilterCompute.SetFloat(SATURATION, saturationSlider.value);
        DispathKernel(ADJUST_COLOR);
    }
}
