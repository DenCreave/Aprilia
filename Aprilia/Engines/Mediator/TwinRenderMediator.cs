using System.ComponentModel;
using Aprilia.Engines.Glance;
using Aprilia.Models;

namespace Aprilia.Engines.Mediator;

public class TwinRenderMediator
{
    private readonly GlanceState _glanceState;
    private readonly CameraState _cameraState;

    public TwinRenderMediator(GlanceState glanceState, CameraState cameraState)
    {
        _glanceState = glanceState;
        _cameraState = cameraState;
        
        _glanceState.PropertyChanged += OnGlanceStateChanged;
    }

    private void OnGlanceStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GlanceState.StrainProgress))
        {
            _cameraState.StrainProgress = _glanceState.StrainProgress;
        }
        
        if (e.PropertyName == nameof(GlanceState.CurrentFocus))
        {
            _cameraState.CurrentIndex = _glanceState.CurrentFocus;
        }
    }
}