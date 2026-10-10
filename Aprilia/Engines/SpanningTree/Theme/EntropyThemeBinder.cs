using System.Collections.Generic;
using System.Linq;
using Aprilia.Engines.SpanningTree.DotMap;
using Aprilia.Engines.SpanningTree.DotMap.Generic;
using Aprilia.Engines.SpanningTree.TorrentLayer;
using Aprilia.Engines.SpanningTree.TorrentLayer.Entropy;
using Aprilia.Interfaces;

namespace Aprilia.Engines.SpanningTree.Theme;

public class EntropyThemeBinder: IThemeBinder
{
    private readonly DotMapLoader _mapLoader;
    private readonly TorrentLayerLoader _layerLoader;
    public ThemeGroup ThemeGroup => ThemeGroup.Entropy;

    public EntropyThemeBinder(DotMapLoader mapLoader, TorrentLayerLoader layerLoader)
    {
        _mapLoader = mapLoader;
        _layerLoader = layerLoader;
    }

    public Animap[] ElicitateAnimap()
    {
        List<Animap> tmp = new();
        int dotCount = 300;
    
        var particleMap = (GenericParticle)_mapLoader.Maps.First(x => x.Group == ThemeGroup.Generic && x.Name == DMName.Particle);
        var entropyLayer = (EntropyTL)_layerLoader.CreateNewLayer(ThemeGroup.Entropy, TLName.Entropy);
        particleMap.Configure(x => x.DotLimit = dotCount);
        
        int[] primeBasesX = { 2, 5, 11, 17 };
        int[] primeBasesY = { 3, 7, 3, 13 };

        for (int i = 0; i < primeBasesX.Length; i++)
        {
            tmp.Add(new Animap() 
            {
                TorrentLayer = entropyLayer.Configure(x => 
                {
                    x.Noise.NoiseInstances = dotCount;
                    x.Noise.BaseX = primeBasesX[i];
                    x.Noise.BaseY = primeBasesY[i];
                }), 
            
                Graph = particleMap.GenerateVertices()
            }); 
        }

        return tmp.ToArray();
    }
}