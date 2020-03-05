using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetBuilder : MonoBehaviour
{
    private float verticalStride;
    private float horizontalStride;
    [SerializeField] private Image neuronImage;
    [SerializeField] private Image linkImage;
    [SerializeField] private Vector2 predefinedOffset;
    private RectTransform rectTransform;

    private Image[] inputLayer;
    private Image[][] hiddenLayers;
    private Image[] outputLayer;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void DraweNetUI(DNA.DnaTopology topology)
    {
        verticalStride = (rectTransform.rect.height - 100F) / topology.neuronsPerHiddenLayer;
        horizontalStride = (rectTransform.rect.width - 50F) / (topology.hiddenLayerCount + 2);
        inputLayer = new Image[DNA.INPUT_COUNT];
        hiddenLayers = new Image[topology.hiddenLayerCount][];
        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            hiddenLayers[i] = new Image[topology.neuronsPerHiddenLayer];
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
        float startX = (rectTransform.rect.width / 2F) - (((topology.hiddenLayerCount + 1) / 2F) * horizontalStride);
        float startY = (rectTransform.rect.height / 2F) - ((DNA.INPUT_COUNT / 2F) * verticalStride);
        float offset = verticalStride / 2;
        for (int i = 0; i < DNA.INPUT_COUNT; i++)
        {
            inputLayer[i] = DrawNeuronUI(startX, startY + offset + (i * verticalStride));
        }

        startY = (rectTransform.rect.height / 2F) - ((topology.neuronsPerHiddenLayer / 2F) * verticalStride) + offset;

        for (int i = 0; i < topology.hiddenLayerCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                hiddenLayers[i][j] = DrawNeuronUI((startX + horizontalStride * (i + 1)), startY + (j * verticalStride));
            }
        }

        startY = (rectTransform.rect.height / 2F) - ((DNA.OUTPUT_COUNT / 2F) * verticalStride) + offset;
        startX += (topology.hiddenLayerCount + 1) * horizontalStride;

        for (int i = 0; i < DNA.OUTPUT_COUNT; i++)
        {
            outputLayer[i] = DrawNeuronUI(startX, startY + (i * verticalStride));
        }
    }

    private void DrawLinks(DNA.DnaTopology topology)
    {
        for (int i = 0; i < DNA.INPUT_COUNT; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                TUIUtils.GetInstance().DrawSpriteLine(inputLayer[i].rectTransform.position, hiddenLayers[0][j].rectTransform.position, 1.25F, linkImage, transform);
            }
        }

        for (int i = 1; i < topology.hiddenLayerCount; i++)
        {
            for (int j = 0; j < topology.neuronsPerHiddenLayer; j++)
            {
                for (int k = 0; k < topology.neuronsPerHiddenLayer; k++)
                {
                    TUIUtils.GetInstance().DrawSpriteLine(hiddenLayers[i - 1][j].rectTransform.position, hiddenLayers[i][k].rectTransform.position, 1.25F, linkImage, transform);
                }
            }
        }

        for (int i = 0; i < topology.neuronsPerHiddenLayer; i++)
        {
            for (int j = 0; j < DNA.OUTPUT_COUNT; j++)
            {
                TUIUtils.GetInstance().DrawSpriteLine(hiddenLayers[topology.hiddenLayerCount - 1][i].rectTransform.position, outputLayer[j].rectTransform.position, 1.25F, linkImage, transform);
            }
        }
    }
}