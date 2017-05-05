﻿using System;
using System.Diagnostics;
using Avalonia.Media;
using Avalonia.Platform;

namespace Avalonia.Rendering
{
    public class RendererBase
    {
        private static readonly Typeface s_fpsTypeface = new Typeface("Arial", 18);
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _framesThisSecond;
        private int _fps;
        private FormattedText _fpsText;
        private TimeSpan _lastFpsUpdate;

        public RendererBase()
        {
            _fpsText = new FormattedText
            {
                Typeface = new Typeface(null, 18),
            };
        }

        protected void RenderFps(IDrawingContextImpl context, Rect clientRect, bool incrementFrameCount)
        {
            var now = _stopwatch.Elapsed;
            var elapsed = now - _lastFpsUpdate;

            if (incrementFrameCount)
            {
                ++_framesThisSecond;
            }

            if (elapsed.TotalSeconds > 1)
            {
                _fps = (int)(_framesThisSecond / elapsed.TotalSeconds);
                _framesThisSecond = 0;
                _lastFpsUpdate = now;
            }

            _fpsText.Text = string.Format("FPS: {0:000}", _fps);
            var size = _fpsText.Measure();
            var rect = new Rect(clientRect.Right - size.Width, 0, size.Width, size.Height);

            context.Transform = Matrix.Identity;
            context.FillRectangle(Brushes.Black, rect);
            context.DrawText(Brushes.White, rect.TopLeft, _fpsText.PlatformImpl);
        }
    }
}