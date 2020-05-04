using System;
using System.Numerics;
using YAFC.Model;
using YAFC.UI;

namespace YAFC
{
    public abstract class ProjectPage : IGui
    {
        protected ProjectPage()
        {
            headerContent = new ImGui(this, default, RectAllocator.LeftAlign);
            bodyContent = new ImGui(this, default, RectAllocator.LeftAlign, true);
        }

        protected readonly ImGui headerContent;
        protected readonly ImGui bodyContent;
        private float contentWidth, headerHeight, contentHeight;
        private Vector2 maxScroll;
        private Vector2 _scroll;
        
        public abstract Icon icon { get; }
        public abstract string header { get; }

        public abstract void BuildHeader(ImGui gui);
        public abstract void BuildContent(ImGui gui);

        public virtual void Rebuild(bool visualOnly = false)
        {
            headerContent.Rebuild(); 
            bodyContent.Rebuild();
        }

        public void Build(ImGui gui, float height)
        {
            if (gui.action == ImGuiAction.Build)
            {
                gui.spacing = 0f;
                var width = gui.width;
                var position = gui.AllocateRect(0f, 0f, 0f).Position;
                var headerSize = headerContent.CalculateState(width, gui.pixelsPerUnit);
                contentWidth = headerSize.X;
                headerHeight = headerSize.Y;
                var headerRect = gui.AllocateRect(width, headerHeight);
                position.Y += headerHeight;
                var contentSize = bodyContent.CalculateState(width, gui.pixelsPerUnit);
                if (contentSize.X > contentWidth)
                    contentWidth = contentSize.X;
                contentHeight = contentSize.Y;
                var bodyRect = gui.AllocateRect(width, height - headerHeight);
                
                maxScroll = new Vector2(MathF.Max(0f, contentWidth-width), MathF.Max(0f, contentHeight+headerHeight-height));
                scroll = scroll;
                
                headerContent.offset = new Vector2(scroll.X, 0);
                bodyContent.offset = new Vector2(scroll.X, scroll.Y);
                gui.DrawPanel(headerRect, headerContent);
                gui.DrawPanel(bodyRect, bodyContent);
            }
        }

        protected virtual void BuildOverlays(ImGui gui) {}

        public Vector2 scroll
        {
            get => _scroll;
            set
            {
                var sx = MathUtils.Clamp(value.X, 0f, maxScroll.X);
                var sy = MathUtils.Clamp(value.Y, 0f, maxScroll.Y);
                var val = new Vector2(sx, sy);
                if (val != _scroll)
                {
                    _scroll = val;
                    headerContent?.parent.Rebuild();
                }
            }
        }

        public void Build(ImGui gui)
        {
            if (gui == headerContent)
                BuildHeader(gui);
            else if (gui == bodyContent)
                BuildContent(gui);
        }
    }
}