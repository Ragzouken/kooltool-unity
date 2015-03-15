using UnityEngine;
using System.Collections;

public interface IDrawing
{
    void Blit(Point offset, Sprite image, bool subtract);

    void Fill(Point pixel, Color color);
    IEnumerator Fill(Point pixel, Color color, int chunksize);

    bool Sample(Point pixel, out Color color);

    void Apply();
}
