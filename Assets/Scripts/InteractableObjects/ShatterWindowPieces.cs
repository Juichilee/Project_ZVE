using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterWindowPieces : MonoBehaviour
{
    public AudioClip shatter;
    private AudioSource audioSource;

    private Transform allwindowPieces;
    
    private Transform windowPiece1Transform;
    private Transform windowPiece2Transform;
    private Transform windowPiece3Transform;
    private Transform windowPiece4Transform;
    private Transform windowPiece5Transform;
    private Transform windowPiece6Transform;
    private Transform windowPiece7Transform;
    private Transform windowPiece8Transform;
    private Transform windowPiece9Transform;
    
    private GameObject windowPiece1;
    private GameObject windowPiece2;
    private GameObject windowPiece3;
    private GameObject windowPiece4;
    private GameObject windowPiece5;
    private GameObject windowPiece6;
    private GameObject windowPiece7;
    private GameObject windowPiece8;
    private GameObject windowPiece9;

    private Rigidbody windowPiece1Rigidbody;
    private Rigidbody windowPiece2Rigidbody;
    private Rigidbody windowPiece3Rigidbody;
    private Rigidbody windowPiece4Rigidbody;
    private Rigidbody windowPiece5Rigidbody;
    private Rigidbody windowPiece6Rigidbody;
    private Rigidbody windowPiece7Rigidbody;
    private Rigidbody windowPiece8Rigidbody;
    private Rigidbody windowPiece9Rigidbody;

    private bool alreadyShattered = false;
    private float objectDestructionValue = 3f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        allwindowPieces = transform;
        alreadyShattered = false;
        prepareObjects();
    }

    void OnTriggerEnter(Collider c)
    {
        if (alreadyShattered == false && c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player" || c.attachedRigidbody.gameObject.tag == "Projectile" || c.attachedRigidbody.gameObject.tag == "Enemy")
            {
                shatterObject(windowPiece1, windowPiece1Rigidbody);
                shatterObject(windowPiece2, windowPiece2Rigidbody);
                shatterObject(windowPiece3, windowPiece3Rigidbody);
                shatterObject(windowPiece4, windowPiece4Rigidbody);
                shatterObject(windowPiece5, windowPiece5Rigidbody);
                shatterObject(windowPiece6, windowPiece6Rigidbody);
                shatterObject(windowPiece7, windowPiece7Rigidbody);
                shatterObject(windowPiece8, windowPiece8Rigidbody);
                shatterObject(windowPiece9, windowPiece9Rigidbody);
                audioSource.PlayOneShot(shatter);
                alreadyShattered = true;
            }
        }
    }
    
    private void prepareObjects()
    {
        // Window Piece 1
        windowPiece1Transform = allwindowPieces.GetChild(0);
        windowPiece1 = windowPiece1Transform.gameObject;
        windowPiece1Rigidbody = windowPiece1.GetComponent<Rigidbody>();
        // Window Piece 2
        windowPiece2Transform = allwindowPieces.GetChild(1);
        windowPiece2 = windowPiece2Transform.gameObject;
        windowPiece2Rigidbody = windowPiece2.GetComponent<Rigidbody>();
        // Window Piece 3
        windowPiece3Transform = allwindowPieces.GetChild(2);
        windowPiece3 = windowPiece3Transform.gameObject;
        windowPiece3Rigidbody = windowPiece3.GetComponent<Rigidbody>();
        // Window Piece 4
        windowPiece4Transform = allwindowPieces.GetChild(3);
        windowPiece4 = windowPiece4Transform.gameObject;
        windowPiece4Rigidbody = windowPiece4.GetComponent<Rigidbody>();
        // Window Piece 5
        windowPiece5Transform = allwindowPieces.GetChild(4);
        windowPiece5 = windowPiece5Transform.gameObject;
        windowPiece5Rigidbody = windowPiece5.GetComponent<Rigidbody>();
        // Window Piece 6
        windowPiece6Transform = allwindowPieces.GetChild(5);
        windowPiece6 = windowPiece6Transform.gameObject;
        windowPiece6Rigidbody = windowPiece6.GetComponent<Rigidbody>();
        // Window Piece 7
        windowPiece7Transform = allwindowPieces.GetChild(6);
        windowPiece7 = windowPiece7Transform.gameObject;
        windowPiece7Rigidbody = windowPiece7.GetComponent<Rigidbody>();
        // Window Piece 8
        windowPiece8Transform = allwindowPieces.GetChild(7);
        windowPiece8 = windowPiece8Transform.gameObject;
        windowPiece8Rigidbody = windowPiece8.GetComponent<Rigidbody>();
        // Window Piece 9
        windowPiece9Transform = allwindowPieces.GetChild(8);
        windowPiece9 = windowPiece9Transform.gameObject;
        windowPiece9Rigidbody = windowPiece9.GetComponent<Rigidbody>();
    }

    private void shatterObject(GameObject windowPieceGameObject, Rigidbody windowPieceRigidbody)
    {
        windowPieceRigidbody.isKinematic = false;
        windowPieceRigidbody.useGravity = true;
        Destroy(windowPieceGameObject, objectDestructionValue);
    }
}
