using UnityEngine;
using UnityEngine.UI;

public class UINetBuilder : MonoBehaviour
{
    private float[] verticalStride;
    private float horizontalStride;
    [SerializeField] private Image neuronImage = null;
    [SerializeField] private Image linkImage = null;
    [SerializeField] private Vector2 predefinedOffset = Vector2.zero;
    private RectTransform rectTransform;

    private Image[] inputLayer;
    private Image[][] hiddenLayers;
    private Image[] outputLayer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawNetUI(DNA.DnaTopology topology)
    {
        verticalStride = new float[topology.hiddenLayerCount];
        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            verticalStride[i] = (rectTransform.rect.height - 140F) / topology.neuronsAtLayer[i];
        }

        horizontalStride = (rectTransform.rect.width - 60F) / (topology.hiddenLayerCount + 2);
        inputLayer = new Image[DNA.INPUT_COUNT];
        hiddenLayers = new Image[topology.hiddenLayerCount][];
        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            hiddenLayers[i] = new Image[topology.neuronsAtLayer[i]];
        }
        outputLayer = new Image[DNA.OUTPUT_COUNT];

        DrawNeurons(topology);
        DrawLinks(topology);
    }

    private Image DrawNeuronUI(float x, float y)
    {
        Image currentNeuron = Instantiate(neuronImage);
        currentNeuron.transform.SetParent(this.transform);
        currentNeuron.rectTransform.anchoredPosition = new Vector2(x, y) + predefinedOffset;
        return currentNeuron;
    }

    private void DrawNeurons(DNA.DnaTopology topology)
    {
        float currentStride = ((rectTransform.rect.height - 140F) / DNA.INPUT_COUNT);
        float startX = (rectTransform.rect.width / 2F) - (((topology.hiddenLayerCount + 1) / 2F) * horizontalStride);
        float startY = (rectTransform.rect.height / 2F) - ((DNA.INPUT_COUNT / 2F) * currentStride);
        float offset = currentStride / 2;
        for (int i = 0; i < DNA.INPUT_COUNT; i++)
        {
            inputLayer[i] = DrawNeuronUI(startX, startY + offset + (i * currentStride));
        }

        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            startY = (rectTransform.rect.height / 2F) - ((topology.neuronsAtLayer[i] / 2F) * verticalStride[i]);
            offset = verticalStride[i] / 2;
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                hiddenLayers[i][j] = DrawNeuronUI((startX + horizontalStride * (i + 1)), startY + offset + (j * verticalStride[i]));
            }
        }

        currentStride = ((rectTransform.rect.height - 140F) / DNA.OUTPUT_COUNT);
        startY = (rectTransform.rect.height / 2F) - ((DNA.OUTPUT_COUNT / 2F) * currentStride);
        startX += (topology.hiddenLayerCount + 1) * horizontalStride;
        offset = currentStride / 2;
        for (int i = 0; i < DNA.OUTPUT_COUNT; i++)
        {
            outputLayer[i] = DrawNeuronUI(startX, startY + offset + (i * currentStride));
        }
    }

    private void DrawLinks(DNA.DnaTopology topology)
    {
        for (int i = 0; i < DNA.INPUT_COUNT; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[0]; j++)
            {
                TUtilsUI.GetInstance().DrawSpriteLine(inputLayer[i].rectTransform.position, hiddenLayers[0][j].rectTransform.position, 1.25F, linkImage, transform);
            }
        }

        for (int i = 0; i < topology.hiddenLayerCount - 1; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                {
                    TUtilsUI.GetInstance().DrawSpriteLine(hiddenLayers[i][j].rectTransform.position, hiddenLayers[i + 1][k].rectTransform.position, 1.25F, linkImage, transform);
                }
            }
        }

        for (int i = 0; i < topology.neuronsAtLayer[topology.hiddenLayerCount - 1]; i++)
        {
            for (int j = 0; j < DNA.OUTPUT_COUNT; j++)
            {
                TUtilsUI.GetInstance().DrawSpriteLine(hiddenLayers[topology.hiddenLayerCount - 1][i].rectTransform.position, outputLayer[j].rectTransform.position, 1.25F, linkImage, transform);
            }
        }
    }
}