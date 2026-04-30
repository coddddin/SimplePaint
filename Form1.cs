using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace SimplePaint
{
    public partial class Form1 : Form
    {
        enum ToolType { Line, Rectangle, Circle } // 사용할 도형 타입
        private Bitmap canvasBitmap; // 실제 그림이 저장되는 비트맵
        private Graphics canvasGraphics; // 비트맵 위에 그리기 위한객체

        private bool isDrawing = false; // 현재 드래그 중인지 여부
        private Point startPoint; // 드래그 시작점 (비트맵 좌표)
        private Point endPoint; // 드래그 끝점 (비트맵 좌표)

        private ToolType currentTool = ToolType.Line; // 현재 선택된 도형
        private Color currentColor = Color.Black; // 현재 색상
        private int currentLineWidth = 2; // 현재 선 두께

        // 런타임으로 추가하는 스크롤 패널 — 기존 기능은 그대로 유지하면서 스크롤/줌을 제공합니다.
        private Panel canvasPanel;

        // 확대/축소 상태 (기본 1.0 = 100%)
        private double currentZoom = 1.0;
        private const double ZoomStep = 1.25;
        private const double MaxZoom = 8.0;
        private const double MinZoom = 0.1;

        public Form1()
        {
            InitializeComponent();

            // 캔버스 초기화 (기존 동작 유지)
            canvasBitmap = new Bitmap(picCanvas.Width, picCanvas.Height);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.Clear(Color.White); // 캔버스를 흰색으로 초기화

            // PictureBox와 기존 이벤트는 유지하고, 런타임에서 PictureBox 를 Panel 안으로 넣어 스크롤을 제공
            // (디자이너에 있는 picCanvas를 제거하지 않고 위치/크기 정보만 사용해 패널을 생성)
            var originalLocation = picCanvas.Location;
            var originalSize = picCanvas.Size;

            canvasPanel = new Panel
            {
                Location = originalLocation,
                Size = originalSize,
                AutoScroll = true
            };

            // 폼에서 picCanvas 제거 후 패널에 넣기
            this.Controls.Remove(picCanvas);
            picCanvas.Location = new Point(0, 0);
            canvasPanel.Controls.Add(picCanvas);
            this.Controls.Add(canvasPanel);

            // --- 핵심 변경: Paint 이벤트가 제거되어 점선 미리보기가 사라지는 문제 해결 ---
            // PictureBox를 패널로 옮긴 후에도 Paint 이벤트가 확실히 연결되도록 합니다.
            picCanvas.Paint += PicCanvas_Paint;

            // PictureBox에 비트맵 바인딩 — 기존 동작과 동일하게 보관
            picCanvas.Image = canvasBitmap;
            // StretchImage로 두고 PictureBox 크기를 줌에 따라 변경하면 시각적 확대/축소가 동작합니다.
            picCanvas.SizeMode = PictureBoxSizeMode.StretchImage;

            // 마우스/페인트 이벤트는 기존대로 사용 (디자이너에서 이미 연결되어 있을 가능성 있음)
            // (기존 점선 미리보기 동작을 건드리지 않도록 그대로 둠)

            // 색상/트랙바 초기값 유지
            cmbColor.SelectedIndex = 0;
            trbLineWidth.Minimum = 1;
            trbLineWidth.Maximum = 10; // 확장 여지
            trbLineWidth.Value = 2;
            currentLineWidth = trbLineWidth.Value;

            // 초기 줌(1.0) 적용
            ApplyZoomAndRefresh();
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Line;
        }

        private void btnRectangle_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Rectangle;
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
            currentTool = ToolType.Circle;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            // ShowDialog가 중복 호출되던 문제를 제거하고, 선택한 이미지를 '캔버스 비트맵'으로 사용하도록 변경
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (Image img = Image.FromFile(ofd.FileName))
                    {
                        // 기존 비트맵/그래픽 해제
                        canvasGraphics?.Dispose();
                        canvasBitmap?.Dispose();

                        // 요구사항: 외부 이미지 원본 크기로 캔버스 생성
                        canvasBitmap = new Bitmap(img.Width, img.Height);
                        canvasGraphics = Graphics.FromImage(canvasBitmap);
                        canvasGraphics.Clear(Color.White);
                        canvasGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        // 원본 이미지 그대로 그려서 캔버스로 사용
                        canvasGraphics.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));

                        // 줌 리셋(또는 유지 원하면 주석 처리)
                        currentZoom = 1.0;

                        ApplyZoomAndRefresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"이미지 열기 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (canvasBitmap == null)
            {
                MessageBox.Show("저장할 캔버스가 없습니다.", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG 이미지|*.png|JPEG 이미지|*.jpg;*.jpeg|Bitmap 이미지|*.bmp";
                sfd.DefaultExt = "png";
                sfd.AddExtension = true;
                sfd.FileName = "untitled";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                string ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
                try
                {
                    if (ext == ".jpg" || ext == ".jpeg")
                    {
                        // JPEG 저장 시 품질 설정 (예: 90)
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        if (jpgEncoder != null)
                        {
                            using (EncoderParameters encoderParams = new EncoderParameters(1))
                            {
                                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
                                canvasBitmap.Save(sfd.FileName, jpgEncoder, encoderParams);
                            }
                        }
                        else
                        {
                            canvasBitmap.Save(sfd.FileName, ImageFormat.Jpeg);
                        }
                    }
                    else if (ext == ".bmp")
                    {
                        canvasBitmap.Save(sfd.FileName, ImageFormat.Bmp);
                    }
                    else // .png (기본)
                    {
                        canvasBitmap.Save(sfd.FileName, ImageFormat.Png);
                    }

                    MessageBox.Show("저장 완료", "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 저장 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbColor.SelectedIndex)
            {
                case 0: // Black 검정
                    currentColor = Color.Black;
                    break;
                case 1: // Red 빨강
                    currentColor = Color.Red;
                    break;
                case 2: // Blue 파랑
                    currentColor = Color.Blue;
                    break;
                case 3: // Green 녹색
                    currentColor = Color.Green;
                    break;
                default:
                    currentColor = Color.Black;
                    break;
            }
        }

        private void trbLineWidth_Scroll(object sender, EventArgs e)
        {

        }

        private void trbLineWidth_ValueChanged(object sender, EventArgs e)
        {
            currentLineWidth = trbLineWidth.Value;
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(
            Math.Min(p1.X, p2.X),
            Math.Min(p1.Y, p2.Y),
            Math.Abs(p1.X - p2.X),
            Math.Abs(p1.Y - p2.Y)
            );
        }
        private void DrawShape(Graphics g, Pen pen, Point p1, Point p2)
        {
            Rectangle rect = GetRectangle(p1, p2);
            switch (currentTool)
            {
                case ToolType.Line:
                    g.DrawLine(pen, p1, p2);
                    break;
                case ToolType.Rectangle:
                    g.DrawRectangle(pen, rect);
                    break;
                case ToolType.Circle:
                    g.DrawEllipse(pen, rect);
                    break;
            }
        }

        // 컨트롤(픽쳐박스) 좌표 -> 비트맵 좌표로 변환 (줌 반영)
        private Point ControlToBitmap(Point controlPoint)
        {
            // picCanvas.ClientPoint (픽쳐박스 기준 좌표) 를 비트맵 좌표로 변환
            return new Point(
                Math.Max(0, (int)(controlPoint.X / currentZoom)),
                Math.Max(0, (int)(controlPoint.Y / currentZoom))
            );
        }

        private void PicCanvas_Paint(object sender, PaintEventArgs e)
        {
            // 기존 점선 미리보기 로직은 그대로 유지하되, 확대(zoom)가 있을 때 시각적으로 일치하도록
            if (!isDrawing) return;
            // 점선 펜 (미리보기용)
            using (Pen previewPen = new Pen(currentColor, currentLineWidth))
            {
                previewPen.DashStyle = DashStyle.Dash;
                // DrawShape expects 비트맵 좌표. 현재 Paint의 그래픽은 '컨트롤 좌표'이므로
                // 비트맵 좌표를 컨트롤(확대된) 좌표로 변환해서 그림.
                Point scaledStart = new Point((int)(startPoint.X * currentZoom), (int)(startPoint.Y * currentZoom));
                Point scaledEnd = new Point((int)(endPoint.X * currentZoom), (int)(endPoint.Y * currentZoom));

                // 임시으로 컨트롤 좌표계에서 그리려면 DrawShape와 동일한 로직을 사용
                Rectangle rect = new Rectangle(
                    Math.Min(scaledStart.X, scaledEnd.X),
                    Math.Min(scaledStart.Y, scaledEnd.Y),
                    Math.Abs(scaledStart.X - scaledEnd.X),
                    Math.Abs(scaledStart.Y - scaledEnd.Y)
                );

                switch (currentTool)
                {
                    case ToolType.Line:
                        e.Graphics.DrawLine(previewPen, scaledStart, scaledEnd);
                        break;
                    case ToolType.Rectangle:
                        e.Graphics.DrawRectangle(previewPen, rect);
                        break;
                    case ToolType.Circle:
                        e.Graphics.DrawEllipse(previewPen, rect);
                        break;
                }
            }
        }

        private void PicCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (canvasBitmap == null) return;

            isDrawing = true; // 드래그 시작
            // 컨트롤 좌표 -> 비트맵 좌표
            startPoint = ControlToBitmap(e.Location);
        }

        private void PicCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return; // 그림 그리기와 상관 없는 마우스 움직임은무시
            endPoint = ControlToBitmap(e.Location); // 현재 위치 갱신 (비트맵 좌표)

            picCanvas.Invalidate(); // 화면 다시 그리기 (미리보기)
        }

        private void PicCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return; // 그림 그리기와 상관 없는 마우스 움직임은무시

            isDrawing = false; // 드래그 종료
            endPoint = ControlToBitmap(e.Location);

            // 실제 비트맵에 도형 그리기 (확정) — 비트맵 좌표계로 그림
            using (Pen pen = new Pen(currentColor, currentLineWidth))
            {
                DrawShape(canvasGraphics, pen, startPoint, endPoint);
            }
            // PictureBox에 바뀐 비트맵을 반영 (이미 바인딩되어 있으므로 Invalidate)
            picCanvas.Invalidate(); // 다시 그려서 결과 반영, Paint 이벤트 발생
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid) return codec;
            }
            return null;
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            SetZoom(currentZoom * ZoomStep);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            SetZoom(currentZoom / ZoomStep);
        }

        private void SetZoom(double newZoom)
        {
            newZoom = Math.Max(MinZoom, Math.Min(MaxZoom, newZoom));
            currentZoom = newZoom;
            ApplyZoomAndRefresh();
        }

        // PictureBox 크기/보여주기 갱신 — 기존 기능(미리보기, 최종 그리기)은 건드리지 않음
        private void ApplyZoomAndRefresh()
        {
            if (canvasBitmap == null)
            {
                // 아직 외부 이미지가 없을 때는 picCanvas 초기 크기 기준으로 동작
                picCanvas.Width = (int)Math.Round(picCanvas.Width * currentZoom);
                picCanvas.Height = (int)Math.Round(picCanvas.Height * currentZoom);
            }
            else
            {
                // 실제 캔버스(비트맵) 크기 * 현재 줌으로 PictureBox 크기를 설정하고
                // SizeMode = StretchImage 로 하여 이미지를 확대/축소 표시
                picCanvas.Width = (int)Math.Round(canvasBitmap.Width * currentZoom);
                picCanvas.Height = (int)Math.Round(canvasBitmap.Height * currentZoom);
                picCanvas.Image = canvasBitmap;
            }

            picCanvas.SizeMode = PictureBoxSizeMode.StretchImage;
            picCanvas.Invalidate();
        }
    }
}