using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerInputActions : IInputActionCollection2
{
    public InputActionAsset asset { get; }
    
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Dash;
    private readonly InputAction m_Gameplay_Attack;
    private readonly InputAction m_Gameplay_Ability1;
    private readonly InputAction m_Gameplay_Ability2;
    private readonly InputAction m_Gameplay_Ability3;
    private readonly InputAction m_Gameplay_Ability4;

    public struct GameplayActions
    {
        private PlayerInputActions m_Wrapper;
        
        public GameplayActions(PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        
        public InputAction Move => m_Wrapper.m_Gameplay_Move;
        public InputAction Dash => m_Wrapper.m_Gameplay_Dash;
        public InputAction Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction Ability1 => m_Wrapper.m_Gameplay_Ability1;
        public InputAction Ability2 => m_Wrapper.m_Gameplay_Ability2;
        public InputAction Ability3 => m_Wrapper.m_Gameplay_Ability3;
        public InputAction Ability4 => m_Wrapper.m_Gameplay_Ability4;
        
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                Dash.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                Dash.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                Dash.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnDash;
                Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                Move.started += instance.OnMove;
                Move.performed += instance.OnMove;
                Move.canceled += instance.OnMove;
                Dash.started += instance.OnDash;
                Dash.performed += instance.OnDash;
                Dash.canceled += instance.OnDash;
                Attack.started += instance.OnAttack;
                Attack.performed += instance.OnAttack;
                Attack.canceled += instance.OnAttack;
            }
        }
    }
    
    public GameplayActions Gameplay => new GameplayActions(this);

    public interface IGameplayActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
    }

    public PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""a8b5c3e0-1234-4567-89ab-cdef01234567"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""b8c5d3e0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""c8d5e3f0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""d8e5f3g0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ability1"",
                    ""type"": ""Button"",
                    ""id"": ""e8f5g3h0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ability2"",
                    ""type"": ""Button"",
                    ""id"": ""f8g5h3i0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ability3"",
                    ""type"": ""Button"",
                    ""id"": ""g8h5i3j0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Ability4"",
                    ""type"": ""Button"",
                    ""id"": ""h8i5j3k0-1234-4567-89ab-cdef01234567"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""i8j5k3l0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""j8k5l3m0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""k8l5m3n0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""l8m5n3o0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""m8n5o3p0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""n8o5p3q0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""o8p5q3r0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""p8q5r3s0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""q8r5s3t0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""r8s5t3u0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""s8t5u3v0-1234-4567-89ab-cdef01234567"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ability4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Dash = m_Gameplay.FindAction("Dash", throwIfNotFound: true);
        m_Gameplay_Attack = m_Gameplay.FindAction("Attack", throwIfNotFound: true);
        m_Gameplay_Ability1 = m_Gameplay.FindAction("Ability1", throwIfNotFound: true);
        m_Gameplay_Ability2 = m_Gameplay.FindAction("Ability2", throwIfNotFound: true);
        m_Gameplay_Ability3 = m_Gameplay.FindAction("Ability3", throwIfNotFound: true);
        m_Gameplay_Ability4 = m_Gameplay.FindAction("Ability4", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }
}
