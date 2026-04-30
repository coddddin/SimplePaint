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
        private Point startPoint; // 드래그 시작점
        private Point endPoint; // 드래그 끝점

        private ToolType currentTool = ToolType.Line; // 현재 선택된 도형
        private Color currentColor = Color.Black; // 현재 색상
        private int currentLineWidth = 2; // 현재 선 두께
        public Form1()
        {
            InitializeComponent();
            // 캔버스 초기화
            canvasBitmap = new Bitmap(picCanvas.Width, picCanvas.Height);
            canvasGraphics = Graphics.FromImage(canvasBitmap);
            canvasGraphics.Clear(Color.White); // 캔버스를 흰색으로 초기화

            picCanvas.Image = canvasBitmap; // 그린 그림을 화면(PictureBox)에 표시
            picCanvas.SizeMode = PictureBoxSizeMode.Normal;

            // 마우스 이벤트 연결
            picCanvas.MouseDown += PicCanvas_MouseDown;
            picCanvas.MouseMove += PicCanvas_MouseMove;
            picCanvas.MouseUp += PicCanvas_MouseUp;
            // picCanvas가 다시 그려질 때 PicCanvas_Paint 함수를 실행하도록 연결
            picCanvas.Paint += PicCanvas_Paint;

            // 도형 선택 버튼 이벤트 연결
            btnLine.Click += btnLine_Click;
            btnRectangle.Click += btnRectangle_Click;
            btnCircle.Click += btnCircle_Click;

            // 색상 콤보박스 이벤트 연결
            cmbColor.SelectedIndexChanged += cmbColor_SelectedIndexChanged;
            cmbColor.SelectedIndex = 0; // 기본값: Black

            // 선 두께 트랙바 이벤트 연결
            trbLineWidth.Minimum = 1; // 최소값
            trbLineWidth.Maximum = 10; // 최대값
            trbLineWidth.Value = 2;
            trbLineWidth.ValueChanged += trbLineWidth_ValueChanged;
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
            // ShowDialog()가 중복 호출되어 두 번 열리던 문제 수정:
            // 대화상자는 한 번만 호출하고, 선택되면 해당 파일을 캔버스 비트맵에 그립니다.
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = @"C:\";
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (Image img = Image.FromFile(ofd.FileName))
                    {
                        // 기존 캔버스 비트맵 정리
                        canvasBitmap?.Dispose();

                        // 캔버스 크기에 맞는 비트맵을 새로 만들고 이미지를 그린다 (스트레치)
                        canvasBitmap = new Bitmap(picCanvas.Width, picCanvas.Height);
                        canvasGraphics = Graphics.FromImage(canvasBitmap);
                        canvasGraphics.Clear(Color.White);
                        canvasGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvasGraphics.DrawImage(img, new Rectangle(0, 0, picCanvas.Width, picCanvas.Height));

                        picCanvas.Image = canvasBitmap;
                        picCanvas.SizeMode = PictureBoxSizeMode.Normal;
                        picCanvas.Invalidate();
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
        private void PicCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (!isDrawing) return;
            // 점선 펜 (미리보기용)
            using (Pen previewPen = new Pen(currentColor, currentLineWidth))
            {
                previewPen.DashStyle = DashStyle.Dash;
                DrawShape(e.Graphics, previewPen, startPoint, endPoint);
            }
        }
        private void PicCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true; // 드래그 시작
            startPoint = e.Location; // 시작점 저장
        }

        private void PicCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return; // 그림 그리기와 상관 없는 마우스 움직임은무시
            endPoint = e.Location; // 현재 위치 갱신
                                   
            picCanvas.Invalidate(); // 화면 다시 그리기 (미리보기)
        }

        private void PicCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrawing) return; // 그림 그리기와 상관 없는 마우스 움직임은무시
            
            isDrawing = false; // 드래그 종료
            endPoint = e.Location;
            // 실제 비트맵에 도형 그리기 (확정)
            using (Pen pen = new Pen(currentColor, currentLineWidth))
            {
                DrawShape(canvasGraphics, pen, startPoint, endPoint);
            }
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
    }
}