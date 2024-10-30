using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 360;
    private Vector3 _input;
    public TextMeshProUGUI scoreText;

    public void LookAround()
    {
        GatherInput();
        Look();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        _rb.transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    public void Move()
    {
        _rb.transform.Translate(_input.normalized.magnitude * _speed * Time.deltaTime * transform.forward, Space.World);
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    

    private void Update()
    {
        // LookAround();
        // Move();
    }
}
