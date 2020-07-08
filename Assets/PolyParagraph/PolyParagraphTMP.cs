using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PolyParagraph : MonoBehaviour
{
    
    [Tooltip("This collider defines the generation area")]
    public Collider2D collider2D;

    [TextArea(3, 12 )]
    public string text;

    [Header("Editor Actions")]
    public bool generateLines = false;
    public bool updateText = false;
    public bool generateLorem = false;
    
    [Header("Style")]
    public TextAlignmentOptions _alignment = TextAlignmentOptions.Center;
    public Color color = Color.white;
    public  float fontSize = 1;
    
    [Header("Generated Lines")]
    public List< TextMeshProUGUI> instances = new List<TextMeshProUGUI>( );


    public void Clear()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            instances[i].text = "";
        }
    }
    
    public void SetText( string str)
    {
        Debug.Log("str " + str);
        
        Clear();
        text = str;

        int linesUsed = 0;

        Queue<string> words = new Queue<string>( str.Split(' '));
        for (int i = 0; i < instances.Count; i++)
        {
          
            linesUsed++;


            TextMeshProUGUI textMesh = instances[i];


            while (words.Count > 0)
            {
                string next = words.Peek();
                string prev = textMesh.text;
                textMesh.text += next + " ";
                textMesh.ForceMeshUpdate();
                Debug.Log(textMesh.isTextTruncated);
                if (textMesh.isTextTruncated)
                {
                    textMesh.text = prev;
                    Debug.Log("next " +next );
                    break;
                }
                else
                {
                    words.Dequeue();
                }
            }
        }

        int diff = instances.Count - linesUsed;


        Debug.Log("Diff" + diff);

        if (Application.isPlaying)
        {
            for (int i = 0; i < instances.Count; i++)
                instances[i].transform.position -= offset;

            offset = Vector3.down * diff * fontSize / 2f;

            for (int i = 0; i < instances.Count; i++)
                instances[i].transform.position += offset;
        }

    }
    
    Vector3 offset;


[System.Serializable]
    public   class Line
     {
         public float start = float.NegativeInfinity;
         public float end = float.PositiveInfinity;
         public float y;
         public float z;

         public Vector3 A
         {
             get
             {
                 return  new Vector3(start, y, z);
             }
         }
         
         public Vector3 B
         {
             get
             {
                 return  new Vector3(end, y, z);
             }
         }
         
         public Vector3 Center
         {
             get { return (A + B) / 2; }
         }

         public float Width
         {
             get { return Mathf.Abs(start - end); }
         }
     }


    void ClearLines()
    {

        TextMeshProUGUI[] children =   GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = 0; i < children.Length; i++)
        {
            DestroyImmediate(children[i].gameObject);
        }
        
        instances.Clear();
    }

    TextMeshProUGUI InstantiateText()
    {
        TextMeshProUGUI instance = new GameObject("Text " ).AddComponent<TextMeshProUGUI>();
        instance.transform.SetParent(transform);
        instances.Add(instance);
        instance.overflowMode = TextOverflowModes.Truncate;
        instance.text = "";
        return instance;
    }


    

    public void GenerateTextMeshes()
    {
        ClearLines();
        GenerateLines();

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            TextMeshProUGUI text = InstantiateText();
            text.transform.position = line.Center;
            text.alignment = _alignment;
            text.fontSize = fontSize;
            text.color = color;
            text.rectTransform.sizeDelta = new Vector2(line.Width, fontSize*1.2f);
        }
    }

      List<Line> lines = new List<Line>();


      
      private void OnDrawGizmos()
      {
          GenerateLines();
          
          if (generateLorem)
          {
              generateLorem = false;
              SetText(LoremIpusm);
          }

          if (updateText)
          {
              updateText = false;
              SetText(text);

          }

          if (generateLines)
          {
              generateLines = false;
              GenerateTextMeshes();
              SetText(text);

          }
      }

      private void GenerateLines()
    {
        
        Vector3 topLeft = collider2D.bounds.center + Vector3.Scale(collider2D.bounds.extents, new Vector3(-1, 1, 1));
        Vector3 bottomRight = collider2D.bounds.center + Vector3.Scale(collider2D.bounds.extents, new Vector3(1, -1, 1));

        float dy = fontSize;
        float dx = fontSize/4f;

        if (fontSize <= 0)
            return;
        
        lines = new List<Line>();

        for (float y = topLeft.y - dy / 2; y > bottomRight.y; y -= dy)
        {
            Line line = null;

            for (float x = topLeft.x + dx / 2; x < bottomRight.x; x += dx)
            {

                Vector3 p = collider2D.bounds.center;
                p.x = x;
                p.y = y;

                bool result = collider2D.OverlapPoint(p);

                if (result)
                {
                    if (line == null)
                    {
                        line = new Line();
                        line.y = p.y;
                        line.z = p.z;
                        line.start = p.x;
                        line.end = p.x;

                    }

                    line.start = Mathf.Min(line.start, p.x);
                    line.end = Mathf.Max(line.end, p.x);
                }
                // DebugUtils.DrawSphere(p, dx / 10f, result ? Color.red : Color.green);
            }

            if (line != null)
            {
                lines.Add(line);
            }
        }

        for (int i = 0; i < lines.Count; i++)
        {
            // // DebugUtils.DrawLine(lines[i].A,  box.Center, Color.green );
            // // DebugUtils.DrawLine(lines[i].B,  box.Center, Color.yellow );
            //
             Debug.DrawLine(lines[i].A, lines[i].B, Color.yellow);
        }

    }


        public const string LoremIpusm = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed congue commodo leo consequat ultrices. Duis orci nulla, porta non faucibus ut, volutpat sit amet mi. Donec tincidunt velit placerat orci tincidunt tempor. Sed diam leo, eleifend sed consequat in, pellentesque non neque. Nullam at justo placerat, ullamcorper massa id, pellentesque eros. ";

    
}
