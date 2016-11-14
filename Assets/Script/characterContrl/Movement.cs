using UnityEngine;
using System.Collections;
using System;

public class Movement : MonoBehaviour {
    public Transform _movementController;
    public Transform _lockViewController;
    public Transform _vrObj;
    public Transform _headSetObj;
	public Transform _bodyChild;

    SteamVR_TrackedObject _movementTrackedObj;
    SteamVR_Controller.Device _movementDevice;

    SteamVR_TrackedObject _lockingObj;
    SteamVR_Controller.Device _lockingDevice;

    public float _moveSpeed = 7.0f;
    float _radius = 5.0f;

    Vector3 _lastDir;

    enum ViewMode {
        FirstPerson,
        ThirdPerson,
        Transition,
        None
    };

    Rigidbody _myRb;

    ViewMode _targetMode;
    ViewMode _curMode;

    //camera 
    public float _centerYBias = 1.0f;
    Vector3 _centerBias;
    Vector3 _camBestPos;
    Vector3 _nextFramePrePos;
    float _camBestPosOffsetFactor;
    bool _isFlashingToFloor;

    float _lerpEndDistance = 0.01f;
    float _lerpEndRate = 0.005f;
    public float _lerpTime = 2f;
    int _ballLayerMask;

	//walking
    float _inputFactor;
    bool _isPressing;
	public float _walkRunTransitDur = 0.3f;


    bool _isCamStatic; //watching character moving away without updating the positino of camera
    bool _isMoveUnderControl; // only disable movement

    Vector3 _staticPos;

    //dodging
    Vector3 _lastPress;
    float _pressGapTimer;
    float _gapThreshold = 0.25f;
    public float _dodgeMaxVelo = 8.0f;
    float _dodgeVelo;
    public float _dodgeNormalDur = 1f;
    float _dodgeActualDur;
    float _dodgeTimer = 0.0f;
    Vector3 _dodgeDir;
    bool _isDodging = false;

    //attacking
    bool _isPreparing = false;
    bool _isAttacking = false;
    float _prepareTime = 1.5f;
    float _prepareTimer = 0.0f;
    public GameObject _attackBallPrefab;
    public float _ballBottomHeight = 2.2f;
    float _ballCenterHeight;
    public float _ballPosRatio = 5.0f / 6.0f;
    GameObject _myAttackingBall;
    public GameObject _ballCenterObj;


    //debugging
    public bool _isJinPC = false;


	//jin perimeter set up ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	[Space][Space]
	public GameObject playerAni;
	private Animator anime;

	//jin perimeter set up END ++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    // Use this for initialization
    void Start() {
        _ballCenterHeight = _ballBottomHeight + _attackBallPrefab.transform.lossyScale.y / 2;
        _dodgeActualDur = _dodgeNormalDur;
        _dodgeVelo = _dodgeMaxVelo;
        _pressGapTimer = _gapThreshold;
        _lastPress = Vector3.zero;
        _isMoveUnderControl = true;
        _isCamStatic = false;
        _isFlashingToFloor = false;
        _isPressing = false;
        _inputFactor = 0.0f;
        _myRb = GetComponent<Rigidbody>();
        if (!_myRb) {
            Debug.LogWarning("Character doesn't have a rigidbody attached to it");
        }
        _targetMode = ViewMode.ThirdPerson;
        _curMode = ViewMode.ThirdPerson;
        _camBestPos = Vector3.zero;
        _camBestPosOffsetFactor = 0.05f;
        _centerBias = new Vector3(0, _centerYBias, 0);
        _lastDir = Vector3.zero;
        _movementTrackedObj = _movementController.GetComponent<SteamVR_TrackedObject>();
        _lockingObj = _lockViewController.GetComponent<SteamVR_TrackedObject>();
        _ballLayerMask = ~(1 << LayerMask.NameToLayer(Layers.Ball));

        //
        if (_isJinPC) {
            anime = playerAni.GetComponent<Animator>();
        }
    }
    
    // Update is called once per frame
    void Update() {
        _pressGapTimer += Time.deltaTime;
        if (!_isPressing) {
            InputRegression();
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            SwitchPerspective();
            Debug.Log("CurMode: " + _curMode + "  Target: " + _targetMode);
        }
        if (Input.GetKeyDown(KeyCode.U)) {
            if (_isCamStatic) {
                MakeCamFree();
            } else {
                MakeCamStatic();
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            if (_isMoveUnderControl) {
                TurnOffMoveControll();
            } else {
                TurnOnMoveControll();
            }
        }

        //if (Input.GetKeyDown(KeyCode.U)) {
        //    Debug.Log("CurMode: " + _curMode + "  Target: " + _targetMode);
        //}

        //if (_isPreparing) {
        //    Debug.Log("Preparing");
        //}
        //if (_isAttacking) {
        //    Debug.Log("Attacking");
        //}

        _isPressing = false;
        //character rotation and movement, 3rd camera rotate around character
        if (_movementController.gameObject.activeSelf && _lockViewController.gameObject.activeSelf) {

            if (_curMode == ViewMode.ThirdPerson && _isMoveUnderControl) {
                _movementDevice = SteamVR_Controller.Input((int)_movementTrackedObj.index);
                _lockingDevice = SteamVR_Controller.Input((int)_lockingObj.index);

                if (_movementDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && !(_isPreparing || _isAttacking)) {
                    PressingInput();
                    _isPressing = true;
                    //transform.Translate(transform.forward * Time.deltaTime * _moveSpeed, Space.World);
                }

                //Attack
                if (!_isCamStatic) {
                    if (_movementDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && !_isAttacking) {
                        _myAttackingBall = Instantiate(_attackBallPrefab, GetBallPosition(), Quaternion.identity) as GameObject;
                        _myAttackingBall.GetComponent<weapon>().UpdateCentralPoint(_ballCenterObj);
                        //start playing preparing attacking animation
                    }
                    if (_movementDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger) && !_isAttacking) {
                        _isPreparing = true;
                        _prepareTimer += Time.deltaTime;
                    } else if (_movementDevice.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
                        if (_isPreparing) {
                            _isPreparing = false;
                            _prepareTimer = 0.0f;
                            if (_isAttacking) {
                                //play anim for holding the cannon
                            } else {
                                Destroy(_myAttackingBall);
                                _myAttackingBall = null;
                                //play anim for putting back the cannon, and maybe ball disappear anim
                            }
                        }
                    }
                    if (_prepareTimer >= _prepareTime && _isPreparing) {
                        _isAttacking = true;
                    }

                    if (_isPreparing || _isAttacking) {
                        _ballCenterObj.transform.position = GetBallPosition();
                                                
                        // substitute lucas's implementation with this
                        //_myAttackingBall.transform.position = _ballCenterObj.transform.position;
                    }
                }

                if (_isDodging) {
                    _inputFactor = 0.0f;
                    Debug.Log("Dodge!!");
                    //_dodgeVelo -= _dodgeMaxVelo * Time.deltaTime / _dodgeActualDur;

                    _dodgeTimer += Time.deltaTime;
                    if (_dodgeTimer >= _dodgeActualDur) {
                        _isDodging = false;
                        _dodgeTimer = 0.0f;
                        _dodgeVelo = _dodgeMaxVelo;
                        Debug.Log("Finish Dodging");
                    }
                } else if (_lockingDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
                    if (_movementDevice.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) {
                        Vector3 _dir = new Vector3(_movementDevice.GetAxis().x, 0, _movementDevice.GetAxis().y);
                        if (AngleLessThanThreshold(_dir)) {
                            transform.rotation *= Quaternion.FromToRotation(_lastDir, _dir);
                        }
                        _lastDir = _dir;
                    }
                } else if (!_isCamStatic) {
                    transform.rotation = Quaternion.Euler(0, _headSetObj.rotation.eulerAngles.y, 0);
                }

				if (_movementDevice.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad) && !_isDodging && !(_isPreparing || _isAttacking)) {
                    Vector3 _curDir = new Vector3(_movementDevice.GetAxis().x, 0, _movementDevice.GetAxis().y);
                    if (_pressGapTimer < _gapThreshold) {
						Vector3 _dodgeLocalDir = (_curDir + _lastPress).normalized;

                        Vector3 _headsetForwardWithoutY = _headSetObj.forward;
                        _headsetForwardWithoutY.y = 0.0f;
                        _dodgeDir = Quaternion.FromToRotation(Vector3.forward, _dodgeLocalDir) * _headsetForwardWithoutY;
                        _isDodging = true;
                        _inputFactor = 0.0f;
                        if (Vector3.Dot(_dodgeDir, transform.forward) > 0) {
                            PlayDodgeForwardAnim();
                            transform.rotation = Quaternion.LookRotation(_dodgeDir, Vector3.up);
                            _dodgeActualDur = _dodgeNormalDur;
                        } else {
                            PlayDodgeBackwardAnim();
                            _dodgeActualDur = _dodgeNormalDur / 2.0f;
                        }
                    }
                    _pressGapTimer = 0.0f;
					_lastPress = _curDir;
                }
            }

            _myRb.velocity = transform.forward * _moveSpeed * _inputFactor;
            if (_isDodging) {
                _myRb.velocity += _dodgeVelo * _dodgeDir;
            }
            if (_inputFactor != 0) {
                PlayRunAnim();
            } else {
                PlayStandAnim();
            }

            if (!_isCamStatic) {
                if (_curMode == ViewMode.Transition && _targetMode == ViewMode.FirstPerson) {
					if (Vector3.Distance (_headSetObj.position, transform.position) < _lerpEndDistance) {
						_curMode = ViewMode.FirstPerson;
					}
					RaycastHit _hitInfo;
					if (Physics.Raycast (_headSetObj.position, (transform.position - _headSetObj.position).normalized, out _hitInfo, (transform.position - _headSetObj.position).magnitude)) {
						if (_hitInfo.collider.gameObject.tag == Tags.Player) {
							Vector3 _disablePoint = _hitInfo.point + (_headSetObj.position - _hitInfo.point).normalized * 0.3f;
							if (Vector3.Distance(_disablePoint, _headSetObj.position) < 0.01f){
								foreach (Transform _renderChild in _bodyChild) {
									_renderChild.gameObject.SetActive (false);
								}
							}
						}
					}
                    //Debug.Log("Yeah");
                }
            }

            _isFlashingToFloor = false;

            if (!_isCamStatic) {
                if (_curMode != ViewMode.FirstPerson) {
                    //transform.rotation = Quaternion.Euler(0, _tarRot.rotation.eulerAngles.y, 0); // sync rot for both 3rd cam and 1st cam
                    Vector3 _idealHeadSetPos = -_headSetObj.forward * _radius + transform.position;
                    // the ideal position for 3rd person camera's position when rotation head, ideal here means without obstacle
                    float _realRadius = Vector3.Distance(transform.position, _headSetObj.position);
                    //radius from current frame, which is used for calculating next pre-position before lerping along the line
                    _nextFramePrePos = transform.position + (_idealHeadSetPos - transform.position).normalized * _realRadius;
                    //used radius from last frame and calculating pre-position in next frame which is actually used for lerping

                    if (_curMode == ViewMode.ThirdPerson || _targetMode == ViewMode.ThirdPerson) {
                        //collider raycast for getting point infomation when collision happens
                        //this point is where the camera is going to move to for avoiding obstacles
                        RaycastHit _hitInfo;
						if (Physics.Raycast(transform.position, (_idealHeadSetPos - transform.position).normalized, out _hitInfo, (_idealHeadSetPos - transform.position).magnitude, _ballLayerMask)) {
                            _camBestPos = _hitInfo.point + (transform.position - _idealHeadSetPos) * _camBestPosOffsetFactor;
                            if (_hitInfo.collider.tag == Tags.Ground) {
                                // it is hitting floor, floor is unique, cuz we don't want player can see from the bottom of the floor, so when hitting with floor
                                // and the camera is not lerping back, it should disable lerping and set position directly
                                if (!IsPointBetween(_nextFramePrePos, _camBestPos, transform.position)) {
                                    _isFlashingToFloor = true;
                                }
                            }
                        } else {
                            _camBestPos = _idealHeadSetPos;
                        }
                    } else {
                        _camBestPos = transform.position;
                    }

                    if (!_isFlashingToFloor) {
                        //bias the local position of camera by changing the position of camera's parent
                        _vrObj.position += LerpToTarget(_nextFramePrePos, _camBestPos) - _headSetObj.position;
                    } else {
						//_vrObj.position += LerpToTarget (_headSetObj.position, _camBestPos) - _headSetObj.position;
                        _vrObj.position += _camBestPos - _headSetObj.position;
                    }
                }

                if (_curMode == ViewMode.FirstPerson) {
                    _vrObj.position += transform.position - _headSetObj.position;
                }
                _staticPos = Vector3.zero;
            } else {
                _vrObj.position += _staticPos - _headSetObj.position;
            }

        } else {
            Debug.LogWarning("Cannot find Movement Controller");
        }
    }

    Vector3 GetBallPosition() {
        Vector3 _ballPosUnfixedY = transform.position - (transform.position - _camBestPos) * _ballPosRatio;
        if (_ballPosUnfixedY.y < _ballCenterHeight) {
            _ballPosUnfixedY.y = _ballCenterHeight;
        }
        return _ballPosUnfixedY;
    }

    bool AngleLessThanThreshold(Vector3 _dir) {
        if (Vector3.Angle(_dir, _lastDir) < 20) {
            return true;
        } else {
            return false;
        }
    }

    bool IsPointBetween(Vector3 input, Vector3 point1, Vector3 point2) {
        float _diffX = (input.x - point1.x) * (input.x - point2.x);
        float _diffY = (input.y - point1.y) * (input.y - point2.y);
        float _diffZ = (input.z - point1.z) * (input.z - point2.z);
        if (_diffX <= 0 && _diffY <= 0 && _diffZ <= 0) {
            return true;
        } else {
            return false;
        }
    }

    Vector3 LerpToTarget(Vector3 _origin, Vector3 _tar) {
        float _lerpFactor;
        if (_origin != _tar) {
            _lerpFactor = 1 - Mathf.Pow(_lerpEndRate, Time.deltaTime / _lerpTime);
        } else {
            _lerpFactor = 0.0f;
        }
        return Vector3.Lerp(_origin, _tar, _lerpFactor);
    }

    void InputRegression() {
        if (_inputFactor != 0) {
            if (_inputFactor > 0) {
				_inputFactor -= Time.deltaTime / _walkRunTransitDur;
                if (_inputFactor <= 0) {
                    _inputFactor = 0;
                }
            } else {
				_inputFactor += Time.deltaTime / _walkRunTransitDur;
                if (_inputFactor >= 0) {
                    _inputFactor = 0;
                }
            }
        }
    }

    void PressingInput() {
        if (_inputFactor < 1) {
			_inputFactor += Time.deltaTime / _walkRunTransitDur;
            if (_inputFactor >= 1) {
                _inputFactor = 1;
            }
        }
    }

    void PlayDodgeForwardAnim() {
        if (_isJinPC) {
            anime.SetTrigger("flip");
        }
    }

    void PlayDodgeBackwardAnim() {
        if (_isJinPC) {
            anime.SetTrigger ("dogge");
        }
    }

    void PlayRunAnim() {
        if (_isJinPC) {
            anime.SetBool("run_now", true);
        }
    }

    void PlayStandAnim() {
        if (_isJinPC) {
            anime.SetBool("run_now", false);
        }
    }

    public void SwitchPerspective() {
        _targetMode = (_targetMode == ViewMode.ThirdPerson) ? ViewMode.FirstPerson : ViewMode.ThirdPerson;
        if (_targetMode == ViewMode.ThirdPerson) {
            _curMode = ViewMode.ThirdPerson;
			foreach (Transform _renderChild in _bodyChild) {
				_renderChild.gameObject.SetActive (true);
			}
        } else {
            _curMode = ViewMode.Transition;
        }
    }



    public void MakeCamStatic() {
        _isCamStatic = true;
        _staticPos = _headSetObj.position;
    }

    public void MakeCamFree() {
        _isCamStatic = false;
    }



	// 3th personal view with out move
    public void TurnOffMoveControll() {
        _isMoveUnderControl = false;
    }

    public void TurnOnMoveControll() {
        _isMoveUnderControl = true;
    }
    
    public bool IsRunning() {
        if (_inputFactor != 0) {
            return true;
        } else {
            return false;
        }
    }

	//Vector3 GetMyForwardWithoutY(Vector3 target, Vector3 origin) {
	//    Vector3 _forward3D = target - origin;
	//    _forward3D.y = 0;
	//    return _forward3D;
	//}

    //bool LessThenThreshold(Vector3 _dir) {
    //    if (Vector3.Angle(Vector3.forward, _dir) < 45.0f) {
    //        return true;
    //    } else {
    //        return false;
    //    }
    //}

    //bool IsInTheSameLine(Vector3 point1, Vector3 point2, Vector3 input) {
    //    float ratioX = 0, ratioY = 0, ratioZ = 0;
    //    int flagX = 0, flagY = 0, flagZ = 0;
    //    if (point2.x != point1.x) {
    //        ratioX = (input.x - point1.x) / (point2.x - point1.x);
    //        flagX = 1;
    //    }
    //    if (point2.y != point1.y) {
    //        ratioY = (input.y - point1.y) / (point2.y - point1.y);
    //        flagY = 1;
    //    }
    //    if (point2.z != point1.z) {
    //        ratioZ = (input.z - point1.z) / (point2.z - point1.z);
    //        flagZ = 1;
    //    }
    //    if (flagX * flagY * flagZ == 0) {
    //        if (flagX == 0) {
    //            if (flagY * flagZ == 0) {
    //                return true;
    //            } else if (ratioY == ratioZ){
    //                return true;
    //            } else {
    //                return false;
    //            }
    //        } else {
    //            if (flagY == 0) {
    //                if (flagZ == 0) {
    //                    return true;
    //                } else if (ratioX == ratioZ) {
    //                    return true;
    //                } else {
    //                    return false;
    //                }
    //            } else if (ratioX == ratioY){
    //                return true;
    //            } else {
    //                return false;
    //            }
    //        }
    //    } else {
    //        if (ratioX == ratioY && ratioX != 0 && ratioX == ratioZ) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    }
    //}
}
