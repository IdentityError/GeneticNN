using Assets.Scripts.MachineLearning;
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

    private Image[][] layers;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void DrawNetUI(DNA.DnaTopology topology)
    {
        Clear();
        verticalStride = new float[topology.layerCount];
        for (int i = 0; i < topology.layerCount; i++)
        {
            verticalStride[i] = (rectTransform.rect.height - 140F) / topology.neuronsAtLayer[i];
        }

        horizontalStride = (rectTransform.rect.width - 140F) / (topology.layerCount);

        layers = new Image[topology.layerCount][];
        for (int i = 0; i < topology.layerCount; i++)
        {
            layers[i] = new Image[topology.neuronsAtLayer[i]];
        }

        DrawNeurons(topology);
        DrawLinks(topology);
    }

    //public void DrawNeuralNetUI(Genotype genotype)
    //{
    //    List<Tuple<int, int>> neurons = new List<Tuple<int, int>>();
    //    neurons.Add(Tuple.Create(0, genotype.InputCount));
    //    int number = 0;
    //    int layer = 0;
    //    List<Tuple<int, List<NodeGene>>> currentLayer = new List<Tuple<int, List<NodeGene>>>();
    //    if (genotype.HiddenCount != 0)
    //    {
    //        foreach (NodeGene node in genotype.all)
    //        {
    //            if (node.GetType() == NodeType.HIDDEN)
    //            {
    //                foreach (LinkGene link in node.GetIncomingLinks())
    //                {
    //                    if (link.From().GetType() == NodeType.INPUT)
    //                    {
    //                        number++;
    //                        if (!currentLayer.Contains(node))
    //                        {
    //                            currentLayer.Add(node);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    private Image DrawNeuronUI(float x, float y)
    {
        Image currentNeuron = Instantiate(neuronImage);
        currentNeuron.transform.SetParent(this.transform);
        currentNeuron.rectTransform.anchoredPosition = new Vector2(x, y) + predefinedOffset;
        return currentNeuron;
    }

    private void DrawNeurons(DNA.DnaTopology topology)
    {
        float startX = (rectTransform.rect.width / 2F) - (((topology.layerCount - 1) / 2F) * horizontalStride);
        float offset, startY;
        for (int i = 0; i < topology.layerCount; i++)
        {
            startY = (rectTransform.rect.height / 2F) - ((topology.neuronsAtLayer[i] / 2F) * verticalStride[i]);
            offset = verticalStride[i] / 2;
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                layers[i][j] = DrawNeuronUI((startX + (horizontalStride * i)), startY + offset + (j * verticalStride[i]));
            }
        }
    }

    private void DrawLinks(DNA.DnaTopology topology)
    {
        for (int i = 0; i < topology.layerCount - 1; i++)
        {
            for (int j = 0; j < topology.neuronsAtLayer[i]; j++)
            {
                for (int k = 0; k < topology.neuronsAtLayer[i + 1]; k++)
                {
                    TUtilsUI.GetInstance().DrawSpriteLine(layers[i][j].rectTransform.position, layers[i + 1][k].rectTransform.position, 1.25F, linkImage, transform);
                }
            }
        }
    }

    private void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}