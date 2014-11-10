using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {
	[RequireComponent (typeof(ParticleSystem))]
	/// <summary>
	/// The PlaygroundParticlesC class is a Particle Playground system driven by the Playground Manager (PlaygroundC). A Particle Playground system contains settings and data for altering a Shuriken component.
	/// </summary>
	[ExecuteInEditMode()]
	public class PlaygroundParticlesC : MonoBehaviour {

		/*************************************************************************************************************************************************
			PlaygroundParticlesC variables
		*************************************************************************************************************************************************/

		// Particle Playground settings
		/// <summary>
		/// The particle source method for distributing particles upon birth.
		/// </summary>
		[HideInInspector] public SOURCEC source;
		/// <summary>
		/// Current active state (when using state as source).
		/// </summary>
		[HideInInspector] public int activeState;
		/// <summary>
		/// If emission of particles is active on this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public bool emit = true;
		/// <summary>
		/// Should a particle re-emit when reaching the end of its lifetime?
		/// </summary>
		[HideInInspector] public bool loop = true;
		/// <summary>
		/// Should particles be removed instantly when you set emit to false?
		/// </summary>
		[HideInInspector] public bool clearParticlesOnEmissionStop = false;
		/// <summary>
		/// Should the GameObject of this PlaygroundParticlesC disable when not looping? 
		/// </summary>
		[HideInInspector] public bool disableOnDone = false;
		[HideInInspector] public ONDONE disableOnDoneRoutine;
		/// <summary>
		/// The rate to update this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public int updateRate = 1;
		/// <summary>
		/// Calculate forces on this PlaygroundParticlesC (can be overrided by PlaygroundC.calculate). This will automatically enable/disable when pauseCalculationWhenInvisible is set to true.
		/// </summary>
		[HideInInspector] public bool calculate = true;
		/// <summary>
		/// Calculate the delta movement force of this particle system.
		/// </summary>
		[HideInInspector] public bool calculateDeltaMovement = true;
		/// <summary>
		/// The strength to multiply delta movement with.
		/// </summary>
		[HideInInspector] public float deltaMovementStrength = 10f;
		/// <summary>
		/// The current world object will change its vertices over time. This produces memory garbage with quantity based upon the mesh vertices.
		/// </summary>
		[HideInInspector] public bool worldObjectUpdateVertices = false;
		/// <summary>
		/// The current world object will change its normals over time. This produces memory garbage with quantity based upon the mesh normals.
		/// </summary>
		[HideInInspector] public bool worldObjectUpdateNormals = false;
		/// <summary>
		/// The initial source position when using lifetime sorting of Nearest Neighbor / Nearest Neighbor Reversed.
		/// </summary>
		[HideInInspector] public int nearestNeighborOrigin = 0;
		/// <summary>
		/// The amount of particles within this PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public int particleCount;
		/// <summary>
		/// The percentage to emit of particleCount in bursts from this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public float emissionRate = 1f;									
		/// <summary>
		/// The method to calculate Overflow Offset with.
		/// </summary>
		[HideInInspector] public OVERFLOWMODEC overflowMode = OVERFLOWMODEC.SourceTransform;
		/// <summary>
		/// The offset direction and magnitude when particle count exceeds source count.
		/// </summary>
		[HideInInspector] public Vector3 overflowOffset;									
		/// <summary>
		/// Should source position scattering be applied?
		/// </summary>
		[HideInInspector] public bool applySourceScatter = false;
		/// <summary>
		/// The minimum spread of source position scattering.
		/// </summary>
		[HideInInspector] public Vector3 sourceScatterMin;									
		/// <summary>
		/// The maximum spread of source position scattering.
		/// </summary>
		[HideInInspector] public Vector3 sourceScatterMax;									
		/// <summary>
		/// Sort mode for particle lifetime.
		/// </summary>
		[HideInInspector] public SORTINGC sorting = SORTINGC.Scrambled;						
		/// <summary>
		/// Custom sorting for particle lifetime (when sorting is set to Custom).
		/// </summary>
		[HideInInspector] public AnimationCurve lifetimeSorting;							
		/// <summary>
		/// Minimum size of particles.
		/// </summary>
		[HideInInspector] public float sizeMin = 1f;										
		/// <summary>
		/// Maximum size of particles.
		/// </summary>
		[HideInInspector] public float sizeMax = 1f;										
		/// <summary>
		/// The scale of minimum and maximum particle size.
		/// </summary>
		[HideInInspector] public float scale = 1f;											
		/// <summary>
		/// Minimum initial particle rotation.
		/// </summary>
		[HideInInspector] public float initialRotationMin;									
		/// <summary>
		/// Maximum initial particle rotation.
		/// </summary>
		[HideInInspector] public float initialRotationMax;									
		/// <summary>
		/// Minimum amount to rotate a particle over time.
		/// </summary>
		[HideInInspector] public float rotationSpeedMin;									
		/// <summary>
		/// Maximum amount to rotate a particle over time.
		/// </summary>
		[HideInInspector] public float rotationSpeedMax;									
		/// <summary>
		/// Should the particles rotate towards their movement direction. The rotationNormal will determine from which angle the rotation is based on.
		/// </summary>
		[HideInInspector] public bool rotateTowardsDirection = false;						
		/// <summary>
		/// The rotation direction normal when rotating towards direction (always normalized value).
		/// </summary>
		[HideInInspector] public Vector3 rotationNormal = -Vector3.forward;					
		/// <summary>
		/// The method to apply lifetime values.
		/// </summary>
		[HideInInspector] public VALUEMETHOD lifetimeValueMethod;							
		/// <summary>
		/// The maximum life of a particle in seconds.
		/// </summary>
		[HideInInspector] public float lifetime;											
		/// <summary>
		/// The minimum life of a particle when using lifetimeValueMethod of RandomBetweenTwoValues.
		/// </summary>
		[HideInInspector] public float lifetimeMin = 0;										
		/// <summary>
		/// The offset of lifetime in this particle system.
		/// </summary>
		[HideInInspector] public float lifetimeOffset;										
		/// <summary>
		/// Should lifetime size affect each particle?
		/// </summary>
		[HideInInspector] public bool applyLifetimeSize = true;								
		/// <summary>
		/// The size over lifetime of each particle.
		/// </summary>
		[HideInInspector] public AnimationCurve lifetimeSize;								
		/// <summary>
		/// Should the particles only position on their source (and not apply any forces)?
		/// </summary>
		[HideInInspector] public bool onlySourcePositioning = false;						
		/// <summary>
		/// Should the particles only position by lifetime positioning Vector3AnimationCurves?
		/// </summary>
		[HideInInspector] public bool onlyLifetimePositioning = false;						
		/// <summary>
		/// The lifetime positioning of particles, this will annihilate any forces and only move particles on the X, Y and Z Animation Curves.
		/// </summary>
		[HideInInspector] public Vector3AnimationCurveC lifetimePositioning;				
		/// <summary>
		/// Should scale over time of lifetime positioning apply?
		/// </summary>
		[HideInInspector] public bool applyLifetimePositioningTimeScale = false;			
		/// <summary>
		/// Should scale of position of lifetime positioning apply?
		/// </summary>
		[HideInInspector] public bool applyLifetimePositioningPositionScale = false;		
		/// <summary>
		/// The scale of time for lifetime positioning.
		/// </summary>
		[HideInInspector] public AnimationCurve lifetimePositioningTimeScale;				
		/// <summary>
		/// The scale of positioning for lifetime positioning.
		/// </summary>
		[HideInInspector] public AnimationCurve lifetimePositioningPositionScale;			
		/// <summary>
		/// The overall scale of lifetime positioning.
		/// </summary>
		[HideInInspector] public float lifetimePositioningScale = 1f;						
		/// <summary>
		/// Should lifetime positioning use the direction normal of the source?
		/// </summary>
		[HideInInspector] public bool lifetimePositioningUsesSourceDirection = false;		
		/// <summary>
		/// Should lifetime velocity affect particles?
		/// </summary>
		[HideInInspector] public bool applyLifetimeVelocity = false;						
		/// <summary>
		/// The velocity over lifetime of each particle.
		/// </summary>
		[HideInInspector] public Vector3AnimationCurveC lifetimeVelocity;					
		/// <summary>
		/// The lifetime velocity scale.
		/// </summary>
		[HideInInspector] public float lifetimeVelocityScale = 1f;							
		/// <summary>
		/// Should initial velocity affect particles?
		/// </summary>
		[HideInInspector] public bool applyInitialVelocity = false;							
		/// <summary>
		/// The minimum starting velocity of each particle.
		/// </summary>
		[HideInInspector] public Vector3 initialVelocityMin;								
		/// <summary>
		/// The maximum starting velocity of each particle.
		/// </summary>
		[HideInInspector] public Vector3 initialVelocityMax;								
		/// <summary>
		/// Should initial local velocity affect particles?
		/// </summary>
		[HideInInspector] public bool applyInitialLocalVelocity = false;					
		/// <summary>
		/// The minimum starting velocity of each particle with normal or transform direction.
		/// </summary>
		[HideInInspector] public Vector3 initialLocalVelocityMin;							
		/// <summary>
		/// The maximum starting velocity of each particle with normal or transform direction.
		/// </summary>
		[HideInInspector] public Vector3 initialLocalVelocityMax;							
		/// <summary>
		/// Should the initial velocity shape be applied on particle re/birth?
		/// </summary>
		[HideInInspector] public bool applyInitialVelocityShape = false;					
		/// <summary>
		/// The amount of velocity to apply of the spawning particle's initial/local velocity in form of a Vector3AnimationCurve.
		/// </summary>
		[HideInInspector] public Vector3AnimationCurveC initialVelocityShape;				
		/// <summary>
		/// The scale of initial velocity shape.
		/// </summary>
		[HideInInspector] public float initialVelocityShapeScale = 1f;						
		/// <summary>
		/// Should bending affect particles velocity?
		/// </summary>
		[HideInInspector] public bool applyVelocityBending;									
		/// <summary>
		/// The amount to bend velocity of each particle.
		/// </summary>
		[HideInInspector] public Vector3 velocityBending;									
		/// <summary>
		/// The type of velocity bending.
		/// </summary>
		[HideInInspector] public VELOCITYBENDINGTYPEC velocityBendingType;					
		/// <summary>
		/// The constant force towards gravitational vector.
		/// </summary>
		[HideInInspector] public Vector3 gravity;											
		/// <summary>
		/// The maximum positive- and negative velocity of each particle.
		/// </summary>
		[HideInInspector] public float maxVelocity = 100f;									
		/// <summary>
		/// The force axis constraints of each particle.
		/// </summary>
		[HideInInspector] public PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC();
		/// <summary>
		/// Particles inertia over time.
		/// </summary>
		[HideInInspector] public float damping;
		/// <summary>
		/// The overall scale of velocity.
		/// </summary>
		[HideInInspector] public float velocityScale = 1f;
		/// <summary>
		/// The color over lifetime.
		/// </summary>
		[HideInInspector] public Gradient lifetimeColor;									
		/// <summary>
		/// The colors over lifetime (if Color Source is set to LifetimeColors).
		/// </summary>
		[HideInInspector] public List<PlaygroundGradientC> lifetimeColors = new List<PlaygroundGradientC>();
		/// <summary>
		/// The source to read color from (fallback on Lifetime Color if no source color is available).
		/// </summary>
		[HideInInspector] public COLORSOURCEC colorSource = COLORSOURCEC.Source;			
		/// <summary>
		/// Should the source color use alpha from Lifetime Color instead of the source's original alpha?
		/// </summary>
		[HideInInspector] public bool sourceUsesLifetimeAlpha;								
		/// <summary>
		/// Should the movement of the particle system transform when in local simulation space be compensated for?
		/// </summary>
		[HideInInspector] public bool applyLocalSpaceMovementCompensation = true;			
		/// <summary>
		/// Should particles get a new random size upon rebirth?
		/// </summary>
		[HideInInspector] public bool applyRandomSizeOnRebirth = true;						
		/// <summary>
		/// Should particles get a new random velocity upon rebirth?
		/// </summary>
		[HideInInspector] public bool applyRandomInitialVelocityOnRebirth = true;			
		/// <summary>
		/// Should particles get a new random rotation upon rebirth?
		/// </summary>
		[HideInInspector] public bool applyRandomRotationOnRebirth = true;					
		/// <summary>
		/// Should particles get a new scatter position upon rebirth?
		/// </summary>
		[HideInInspector] public bool applyRandomScatterOnRebirth = false;					
		/// <summary>
		/// Should particles get their initial calculated color upon rebirth? (Can resolve flickering upon rebirth.)
		/// </summary>
		[HideInInspector] public bool applyInitialColorOnRebirth = false;					
		/// <summary>
		/// Should particles get a new random lifetime upon rebirth?
		/// </summary>
		[HideInInspector] public bool applyRandomLifetimeOnRebirth = true;					
		/// <summary>
		/// Should the particle system pause calculation upon becoming invisible?
		/// </summary>
		[HideInInspector] public bool pauseCalculationWhenInvisible = false;				
		/// <summary>
		/// Should the particle system force Play() when GameObject is outside of camera view? (Fix for Shuriken stop rendering.)
		/// </summary>
		[HideInInspector] public bool forceVisibilityWhenOutOfFrustrum = true;				
		/// <summary>
		/// Should each particle's position be synced with main-threaad? Use this when dealing with moving source objects or if you experience a laggy particle movement.
		/// </summary>
		[HideInInspector] public bool syncPositionsOnMainThread = false;					
		/// <summary>
		/// Should the particle system force itself to remain in lockPosition?
		/// </summary>
		[HideInInspector] public bool applyLockPosition = false;							
		/// <summary>
		/// Should the particle system force itself to remain in lockRotation?
		/// </summary>
		[HideInInspector] public bool applyLockRotation = false;							
		/// <summary>
		/// Should the particle system force itself to remain in lockScale?
		/// </summary>
		[HideInInspector] public bool applyLockScale = false;								
		/// <summary>
		/// The locked position is considered local.
		/// </summary>
		[HideInInspector] public bool lockPositionIsLocal = false;							
		/// <summary>
		/// The locked rotation is considered local.
		/// </summary>
		[HideInInspector] public bool lockRotationIsLocal = false;							
		/// <summary>
		/// The locked position.
		/// </summary>
		[HideInInspector] public Vector3 lockPosition = Vector3.zero;						
		/// <summary>
		/// The locked rotation.
		/// </summary>
		[HideInInspector] public Vector3 lockRotation = Vector3.zero;						
		/// <summary>
		/// The locked scale.
		/// </summary>
		[HideInInspector] public Vector3 lockScale = new Vector3(1f,1f,1f);					
		/// <summary>
		/// Should the movementCompensationLifetimeStrength affect local space movement compensation?
		/// </summary>
		[HideInInspector] public bool applyMovementCompensationLifetimeStrength = false;	
		/// <summary>
		/// The strength of movement compensation over particles lifetime
		/// </summary>
		[HideInInspector] public AnimationCurve movementCompensationLifetimeStrength;		
		/// <summary>
		/// The masked amount of particles. The particleMaskTime will determine if the particles should fade in/out.
		/// </summary>
		[HideInInspector] public int particleMask = 0;										
		/// <summary>
		/// The time it takes to mask in/out particles when using particleMask.
		/// </summary>
		[HideInInspector] public float particleMaskTime = 0f;								
		/// <summary>
		/// The speed of stretching to reach full effect.
		/// </summary>
		[HideInInspector] public float stretchSpeed = 1f;									
		/// <summary>
		/// The starting direction of stretching if all intial velocity is zero.
		/// </summary>
		[HideInInspector] public Vector3 stretchStartDirection = Vector3.zero;				
		/// <summary>
		/// Should lifetime stretching be applied?
		/// </summary>
		[HideInInspector] public bool applyLifetimeStretching = false;						
		/// <summary>
		/// The lifetime stretching of stretched particles.
		/// </summary>
		[HideInInspector] public AnimationCurve stretchLifetime;
		/// <summary>
		/// The multithreading method how this particle system should calculate. Use this to bypass the Playground Manager's threadMethod.
		/// </summary>
		[HideInInspector] public ThreadMethodLocal threadMethod;

		// Source Script variables
		/// <summary>
		/// When using Emit() the index will point to the next particle in pool to emit.
		/// </summary>
		[HideInInspector] public int scriptedEmissionIndex;									
		/// <summary>
		/// When using Emit() the passed in position will determine the position for this particle.
		/// </summary>
		[HideInInspector] public Vector3 scriptedEmissionPosition;							
		/// <summary>
		/// When using Emit() the passed in velocity will determine the speed and direction for this particle.
		/// </summary>
		[HideInInspector] public Vector3 scriptedEmissionVelocity;							
		/// <summary>
		/// When using Emit() the passed in color will decide the color for this particle if colorSource is set to COLORSOURCEC.Source.
		/// </summary>
		[HideInInspector] public Color scriptedEmissionColor = Color.white;					

		// Collision detection
		/// <summary>
		/// Determines if particles can collide. Enable this if you want particles to continuously look for colliders of type collisionType (2D/3D). Particle collision will run on main-thread.
		/// </summary>
		[HideInInspector] public bool collision = false;									
		/// <summary>
		/// Should particles affect rigidbodies? The mass determines how much they will affect the rigidobdy.
		/// </summary>
		[HideInInspector] public bool affectRigidbodies = true;								
		/// <summary>
		/// The mass of a particle (calculated in collision with rigidbodies).
		/// </summary>
		[HideInInspector] public float mass = .01f;											
		/// <summary>
		/// The spherical radius of a particle used upon collision.
		/// </summary>
		[HideInInspector] public float collisionRadius = 1f;								
		/// <summary>
		/// The layers these particles will collide with.
		/// </summary>
		[HideInInspector] public LayerMask collisionMask;									
		/// <summary>
		/// The amount a particle will loose of its lifetime on collision.
		/// </summary>
		[HideInInspector] public float lifetimeLoss = 0f;									
		/// <summary>
		/// The amount a particle will bounce on collision.
		/// </summary>
		[HideInInspector] public float bounciness = .5f;									
		/// <summary>
		/// The minimum amount of random bounciness (seen as negative offset from the collided surface's normal direction).
		/// </summary>
		[HideInInspector] public Vector3 bounceRandomMin;									
		/// <summary>
		/// The maximum amount of random bounciness (seen as positive offset from the collided surface's normal direction).
		/// </summary>
		[HideInInspector] public Vector3 bounceRandomMax;									
		/// <summary>
		/// The Playground Colliders of this particle system. A Playground Collider is an infinite collision plane based on a Transform in the scene.
		/// </summary>
		[HideInInspector] public List<PlaygroundColliderC> colliders;						
		/// <summary>
		/// The type of collision. This determines if 2D- or 3D raycasting should be used.
		/// </summary>
		[HideInInspector] public COLLISIONTYPEC collisionType;								
		/// <summary>
		/// Minimum collision depth of Raycast2D.
		/// </summary>
		[HideInInspector] public float minCollisionDepth = 0f;								
		/// <summary>
		/// Maximum collision depth of Raycast2D.
		/// </summary>
		[HideInInspector] public float maxCollisionDepth = 0f;								

		// States (source)
		/// <summary>
		/// The list of States for this PlaygroundParticles. A State is Source data from a texture or mesh which determines where particles will be positioned upon birth.
		/// </summary>
		public List<ParticleStateC> states = new List<ParticleStateC>();					
		
		// Scene objects (source)
		/// <summary>
		/// A mesh as source calculated within the scene.
		/// </summary>
		[HideInInspector] public WorldObject worldObject = new WorldObject();				
		/// <summary>
		/// A skinned mesh as source calculated within the scene.
		/// </summary>
		[HideInInspector] public SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();
		/// <summary>
		/// A transform calculated within the scene.
		/// </summary>
		[HideInInspector] public Transform sourceTransform;									
		
		// Paint
		/// <summary>
		/// The paint source of this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public PaintObjectC paint;										
		
		// Projection
		/// <summary>
		/// The projection source of this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public ParticleProjectionC projection;							
		
		// Manipulators
		/// <summary>
		/// The list of Local Manipulator Objects handled by this PlaygroundParticlesC object.
		/// </summary>
		public List<ManipulatorObjectC> manipulators;										

		// Events
		/// <summary>
		/// List of event objects handled by this PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public List<PlaygroundEventC> events;								

		// Cache
		/// <summary>
		/// Data for each particle.
		/// </summary>
		[NonSerialized] public PlaygroundCache playgroundCache = new PlaygroundCache();
		/// <summary>
		/// The particle pool.
		/// </summary>
		[NonSerialized] public ParticleSystem.Particle[] particleCache; 					

		// Snapshots
		/// <summary>
		/// Saved data of properties (positions, velocities, colors etc.).
		/// </summary>
		[HideInInspector] public List<PlaygroundSave> snapshots = new List<PlaygroundSave>();
		/// <summary>
		/// Should the particle system load stored data from start?
		/// </summary>
		[HideInInspector] public bool loadFromStart = false;								
		/// <summary>
		/// Which data should be loaded (if loadFromStart is true).
		/// </summary>
		[HideInInspector] public int loadFrom = 0;											
		/// <summary>
		/// Should a transition occur whenever a Load is issued?
		/// </summary>
		[HideInInspector] public bool loadTransition = false;								
		/// <summary>
		/// The type of transition to occur whenever a Load is issued.
		/// </summary>
		[HideInInspector] public TRANSITIONTYPEC loadTransitionType;						
		/// <summary>
		/// The time for load transition in seconds.
		/// </summary>
		[HideInInspector] public float loadTransitionTime = 1f;								
		/// <summary>
		/// The storage of position data if this is a snapshot.
		/// </summary>
		[HideInInspector] public PlaygroundCache snapshotData;								
		/// <summary>
		/// The global time the snapshot was made.
		/// </summary>
		[HideInInspector] public float timeOfSnapshot = 0;									
		/// <summary>
		/// Is this particle system a snapshot?
		/// </summary>
		public bool isSnapshot = false;														

		// Components
		/// <summary>
		/// This ParticleSystem (Shuriken) component.
		/// </summary>
		[HideInInspector] public ParticleSystem shurikenParticleSystem;						
		/// <summary>
		/// The id of this PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public int particleSystemId;										
		/// <summary>
		/// The GameObject of a PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public GameObject particleSystemGameObject;						
		/// <summary>
		/// The Transform of a PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public Transform particleSystemTransform;							
		/// <summary>
		/// The Renderer of a PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public Renderer particleSystemRenderer;							
		/// <summary>
		/// The ParticleSystemRenderer of a PlaygroundParticlesC object.
		/// </summary>
		[HideInInspector] public ParticleSystemRenderer particleSystemRenderer2;			
		/// <summary>
		/// The PlaygroundParticlesC that is controlling this particle system.
		/// </summary>
		[HideInInspector] public List<PlaygroundParticlesC> eventControlledBy = new List<PlaygroundParticlesC>();

		// Turbulence
		/// <summary>
		/// The Simplex Turbulence object.
		/// </summary>
		SimplexNoise turbulenceSimplex;
		/// <summary>
		/// The type of turbulence.
		/// </summary>
		[HideInInspector] public TURBULENCETYPE turbulenceType = TURBULENCETYPE.None;
		/// <summary>
		/// The turbulence strength.
		/// </summary>
		[HideInInspector] public float turbulenceStrength = 10f;
		/// <summary>
		/// The turbulence resolution scale. A higher value will generate a more dense grid.
		/// </summary>
		[HideInInspector] public float turbulenceScale = 1f;
		/// <summary>
		/// The turbulence time scale.
		/// </summary>
		[HideInInspector] public float turbulenceTimeScale = 1f;
		/// <summary>
		/// Should Turbulence Lifetime Strength apply?
		/// </summary>
		[HideInInspector] public bool turbulenceApplyLifetimeStrength = false;
		/// <summary>
		/// The Turbulence Lifetime Strength. Use this to control how much turbulence will affect the particle over its lifetime.
		/// </summary>
		[HideInInspector] public AnimationCurve turbulenceLifetimeStrength;

		[HideInInspector] public bool isReadyForThreadedCalculations = false;


		// Internally used variables
		bool inTransition = false;			
		int previousParticleCount = -1;
		float previousEmissionRate = 1f;
		bool cameFromNonCalculatedFrame = false;
		bool cameFromNonEmissionFrame = true;
		bool renderModeStretch = false;
		float previousSizeMin;
		float previousSizeMax;
		float previousInitialRotationMin;
		float previousInitialRotationMax;
		float previousRotationSpeedMin;
		float previousRotationSpeedMax;
		Vector3 previousVelocityMin;
		Vector3 previousVelocityMax;
		Vector3 previousLocalVelocityMin;
		Vector3 previousLocalVelocityMax;
		SORTINGC previousSorting;
		int previousNearestNeighborOrigin;
		VALUEMETHOD previousLifetimeValueMethod;
		float previousLifetime;
		float previousLifetimeMin;
		bool previousEmission = true;
		bool previousLoop;
		float emissionStopped = 0f;
		bool queueEmissionHalt = false;
		int lifetimeColorId = 0;
		System.Random internalRandom01;
		//System.Random internalRandom01;
		//System.Random internalRandom01;
		float lastTimeUpdated = 0f;
		PlaygroundEventParticle eventParticle = new PlaygroundEventParticle();
		[HideInInspector] public float localDeltaTime = 0f;
		[HideInInspector] public int previousActiveState;
		[HideInInspector] public float simulationStarted;
		[HideInInspector] public bool loopExceeded = false;
		[HideInInspector] public int loopExceededOnParticle;
		bool hasEventManipulatorLocal = false;
		bool hasEventManipulatorGlobal = false;
		bool hasSeveralManipulatorEvents = false;
		Quaternion particleSystemInverseRotation;
		bool hasActiveParticles = true;

		PlaygroundParticlesC thisInstance;
		float t;
		Vector3 stPos;
		Quaternion stRot;
		Vector3 stSca;
		Vector3 stDir;
		bool localSpace;
		bool overflow;
		bool skinnedWorldObjectReady;
		bool stateReadyForTextureColor;
		int manipulatorEventCount;
		bool hasEvent;
		bool hasTimerEvent; 

		[HideInInspector] float particleTimescale = 1f;

		/// <summary>
		/// Clones the settings of this Particle Playground system into the passed reference. Note that you additionally need to use CopySaveDataTo() if you want to clone the Snapshots.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public void CopyTo (PlaygroundParticlesC playgroundParticles) {
			
			// Playground variables
			playgroundParticles.source 										= source;
			playgroundParticles.activeState 								= activeState;
			playgroundParticles.emit										= emit;
			playgroundParticles.loop										= loop;
			playgroundParticles.clearParticlesOnEmissionStop				= clearParticlesOnEmissionStop;
			playgroundParticles.disableOnDone								= disableOnDone;
			playgroundParticles.disableOnDoneRoutine						= disableOnDoneRoutine;
			playgroundParticles.updateRate 									= updateRate;
			playgroundParticles.calculate 									= calculate;					
			playgroundParticles.calculateDeltaMovement						= calculateDeltaMovement;
			playgroundParticles.deltaMovementStrength 						= deltaMovementStrength;
			playgroundParticles.worldObjectUpdateVertices					= worldObjectUpdateVertices;
			playgroundParticles.worldObjectUpdateNormals 					= worldObjectUpdateNormals;
			playgroundParticles.nearestNeighborOrigin 						= nearestNeighborOrigin;							
			playgroundParticles.particleCount 								= particleCount;									
			playgroundParticles.emissionRate 								= emissionRate;									
			playgroundParticles.overflowMode 								= overflowMode;	
			playgroundParticles.overflowOffset 								= overflowOffset;									
			playgroundParticles.applySourceScatter							= applySourceScatter;
			playgroundParticles.sourceScatterMin							= sourceScatterMin;
			playgroundParticles.sourceScatterMax							= sourceScatterMax;
			playgroundParticles.sorting 									= sorting;
			playgroundParticles.lifetimeSorting								= new AnimationCurve(lifetimeSorting.keys);					
			playgroundParticles.sizeMin 									= sizeMin;										
			playgroundParticles.sizeMax 									= sizeMax;
			playgroundParticles.scale										= scale;
			playgroundParticles.initialRotationMin 							= initialRotationMin;									
			playgroundParticles.initialRotationMax 							= initialRotationMax;								
			playgroundParticles.rotationSpeedMin 							= rotationSpeedMin;								
			playgroundParticles.rotationSpeedMax 							= rotationSpeedMax;									
			playgroundParticles.rotateTowardsDirection 						= rotateTowardsDirection; 				
			playgroundParticles.rotationNormal 								= rotationNormal;
			playgroundParticles.lifetime 									= lifetime;
			playgroundParticles.lifetimeValueMethod							= lifetimeValueMethod;
			playgroundParticles.lifetimeMin									= lifetimeMin;
			playgroundParticles.lifetimeOffset 								= lifetimeOffset;
			playgroundParticles.applyLifetimeSize							= applyLifetimeSize;
			playgroundParticles.lifetimeSize 								= new AnimationCurve(lifetimeSize.keys);							
			playgroundParticles.onlySourcePositioning 						= onlySourcePositioning;
			playgroundParticles.onlyLifetimePositioning						= onlyLifetimePositioning;
			playgroundParticles.lifetimePositioning							= lifetimePositioning.Clone();
			playgroundParticles.lifetimePositioningScale					= lifetimePositioningScale;
			playgroundParticles.lifetimePositioningUsesSourceDirection		= lifetimePositioningUsesSourceDirection;
			playgroundParticles.lifetimePositioningTimeScale				= new AnimationCurve(lifetimePositioningTimeScale.keys);
			playgroundParticles.lifetimePositioningPositionScale			= new AnimationCurve(lifetimePositioningPositionScale.keys);
			playgroundParticles.applyLifetimePositioningTimeScale			= applyLifetimePositioningTimeScale;
			playgroundParticles.applyLifetimePositioningPositionScale		= applyLifetimePositioningPositionScale;
			playgroundParticles.applyLifetimeVelocity 						= applyLifetimeVelocity;				
			playgroundParticles.lifetimeVelocity 							= lifetimeVelocity.Clone();
			playgroundParticles.lifetimeVelocityScale						= lifetimeVelocityScale;
			playgroundParticles.applyInitialVelocity 						= applyInitialVelocity;						
			playgroundParticles.initialVelocityMin 							= initialVelocityMin;								
			playgroundParticles.initialVelocityMax 							= initialVelocityMax;								
			playgroundParticles.applyInitialLocalVelocity 					= applyInitialLocalVelocity;		
			playgroundParticles.initialLocalVelocityMin 					= initialLocalVelocityMin;							
			playgroundParticles.initialLocalVelocityMax 					= initialLocalVelocityMax;							
			playgroundParticles.applyVelocityBending 						= applyVelocityBending;								
			playgroundParticles.velocityBending 							= velocityBending;
			playgroundParticles.velocityBendingType							= velocityBendingType;
			playgroundParticles.applyInitialVelocityShape					= applyInitialVelocityShape;
			playgroundParticles.initialVelocityShape						= initialVelocityShape.Clone();
			playgroundParticles.initialVelocityShapeScale					= initialVelocityShapeScale;
			playgroundParticles.gravity 									= gravity;											
			playgroundParticles.damping 									= damping;
			playgroundParticles.velocityScale								= velocityScale;
			playgroundParticles.maxVelocity									= maxVelocity;								
			playgroundParticles.lifetimeColor.SetKeys (lifetimeColor.colorKeys, lifetimeColor.alphaKeys);				
			playgroundParticles.colorSource 								= colorSource;				
			playgroundParticles.sourceUsesLifetimeAlpha 					= sourceUsesLifetimeAlpha;
			playgroundParticles.applyLocalSpaceMovementCompensation			= applyLocalSpaceMovementCompensation;
			playgroundParticles.applyRandomSizeOnRebirth					= applyRandomSizeOnRebirth;
			playgroundParticles.applyRandomInitialVelocityOnRebirth			= applyRandomInitialVelocityOnRebirth;
			playgroundParticles.applyRandomRotationOnRebirth				= applyRandomRotationOnRebirth;
			playgroundParticles.applyRandomScatterOnRebirth					= applyRandomScatterOnRebirth;
			playgroundParticles.applyInitialColorOnRebirth					= applyInitialColorOnRebirth;
			playgroundParticles.applyRandomLifetimeOnRebirth				= applyRandomLifetimeOnRebirth;
			playgroundParticles.pauseCalculationWhenInvisible				= pauseCalculationWhenInvisible;
			playgroundParticles.forceVisibilityWhenOutOfFrustrum			= forceVisibilityWhenOutOfFrustrum;
			playgroundParticles.syncPositionsOnMainThread					= syncPositionsOnMainThread;
			playgroundParticles.applyLockPosition							= applyLockPosition;
			playgroundParticles.applyLockRotation							= applyLockRotation;
			playgroundParticles.applyLockScale								= applyLockScale;
			playgroundParticles.lockPositionIsLocal							= lockPositionIsLocal;
			playgroundParticles.lockRotationIsLocal							= lockRotationIsLocal;
			playgroundParticles.lockPosition								= lockPosition;
			playgroundParticles.lockRotation								= lockRotation;
			playgroundParticles.lockScale									= lockScale;
			playgroundParticles.applyMovementCompensationLifetimeStrength	= applyMovementCompensationLifetimeStrength;
			playgroundParticles.movementCompensationLifetimeStrength		= new AnimationCurve(movementCompensationLifetimeStrength.keys);
			playgroundParticles.particleMask								= particleMask;
			playgroundParticles.particleMaskTime							= particleMaskTime;
			playgroundParticles.stretchSpeed								= stretchSpeed;
			playgroundParticles.applyLifetimeStretching						= applyLifetimeStretching;
			playgroundParticles.stretchLifetime								= new AnimationCurve(stretchLifetime.keys);
			playgroundParticles.threadMethod								= threadMethod;

			// Scripted source variables
			playgroundParticles.scriptedEmissionIndex						= scriptedEmissionIndex;
			playgroundParticles.scriptedEmissionPosition					= scriptedEmissionPosition;
			playgroundParticles.scriptedEmissionVelocity					= scriptedEmissionVelocity;
			playgroundParticles.scriptedEmissionColor						= scriptedEmissionColor;

			// Collision detection
			playgroundParticles.collision 									= collision;
			playgroundParticles.affectRigidbodies 							= affectRigidbodies;
			playgroundParticles.mass 										= mass; 
			playgroundParticles.collisionRadius 							= collisionRadius;
			playgroundParticles.collisionMask 								= collisionMask;					
			playgroundParticles.bounciness 									= bounciness;
			playgroundParticles.lifetimeLoss 								= lifetimeLoss;
			playgroundParticles.bounceRandomMin								= bounceRandomMin;
			playgroundParticles.bounceRandomMax								= bounceRandomMax;
			playgroundParticles.collisionType								= collisionType;
			playgroundParticles.minCollisionDepth							= minCollisionDepth;
			playgroundParticles.maxCollisionDepth							= maxCollisionDepth;
			playgroundParticles.colliders									= new List<PlaygroundColliderC>();
			int i;
			for (i = 0; i<colliders.Count; i++)
				playgroundParticles.colliders.Add(colliders[i].Clone());
			
			// States (source)
			playgroundParticles.states 										= new List<ParticleStateC>();
			for (i = 0; i<states.Count; i++) {
				playgroundParticles.states.Add(states[i].Clone());
			}
			// Scene objects (source)
			playgroundParticles.worldObject 								= worldObject.Clone();
			playgroundParticles.skinnedWorldObject 							= skinnedWorldObject.Clone();
			
			playgroundParticles.sourceTransform 							= sourceTransform;
			
			// Paint
			playgroundParticles.paint 										= paint.Clone();

			// Projection
			playgroundParticles.projection 									= projection.Clone();

			// Manipulators
			playgroundParticles.manipulators								= new List<ManipulatorObjectC>();
			for (i = 0; i<manipulators.Count; i++)
				playgroundParticles.manipulators.Add(manipulators[i].Clone());

			// Events
			playgroundParticles.events										= new List<PlaygroundEventC>();
			for (i = 0; i<events.Count; i++)
				playgroundParticles.events.Add(events[i].Clone());

			// Lifetime Colors
			playgroundParticles.lifetimeColors								= new List<PlaygroundGradientC>();
			for (i = 0; i<lifetimeColors.Count; i++) {
				playgroundParticles.lifetimeColors.Add(new PlaygroundGradientC());
				lifetimeColors[i].CopyTo(playgroundParticles.lifetimeColors[i]);
			}

			// Turbulence
			playgroundParticles.turbulenceType								= turbulenceType;
			playgroundParticles.turbulenceApplyLifetimeStrength				= turbulenceApplyLifetimeStrength;
			playgroundParticles.turbulenceLifetimeStrength					= new AnimationCurve(turbulenceLifetimeStrength.keys);
			playgroundParticles.turbulenceScale								= turbulenceScale;
			playgroundParticles.turbulenceStrength							= turbulenceStrength;
			playgroundParticles.turbulenceTimeScale							= turbulenceTimeScale;

			// Other
			playgroundParticles.particleTimescale							= particleTimescale;
			playgroundParticles.thisInstance								= playgroundParticles;

			/*
			playgroundParticles.previousSorting								= previousSorting;
			playgroundParticles.previousNearestNeighborOrigin				= previousNearestNeighborOrigin;
			playgroundParticles.previousEmission							= previousEmission;
			playgroundParticles.previousLoop								= previousLoop;
*/
		}

		/// <summary>
		/// Copies stored data of Snapshots into a particle system (separated from CopyTo() to solve Save/Load overwrite paradox).
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public void CopySaveDataTo (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.snapshots = new List<PlaygroundSave>();
			for (int i = 0; i<snapshots.Count; i++)
				playgroundParticles.snapshots.Add(snapshots[i].Clone());
		}


		/*************************************************************************************************************************************************
			PlaygroundParticlesC functions
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// Emits a single particle at position with velocity and color (Source Mode SOURCEC.Script will be automatically set).
		/// </summary>
		/// <param name="givePosition">Position.</param>
		/// <param name="giveVelocity">Velocity.</param>
		/// <param name="giveColor">Color.</param>
		public int Emit (Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor) {
			source = SOURCEC.Script;
			int returnIndex = scriptedEmissionIndex;
			EmitProcedure(givePosition, giveVelocity, giveColor);
			particleSystemGameObject.SetActive(true);
			return returnIndex;
		}
		
		/// <summary>
		/// Sets emission On or Off.
		/// </summary>
		/// <param name="setEmission">If set to <c>true</c> then emit particles.</param>
		public void Emit (bool setEmission) {
			emit = setEmission;
			if (emit) {
				simulationStarted = PlaygroundC.globalTime;
				loopExceeded = false;
				loopExceededOnParticle = -1;
				Emission(thisInstance, true, true);
				particleSystemGameObject.SetActive(true);
			} else {
				emissionStopped = PlaygroundC.globalTime;
				if (clearParticlesOnEmissionStop)
					InactivateParticles();
			}
			previousEmission = setEmission;
		}

		/// <summary>
		/// Emits number of particles set by quantity, position and minimum - maximum random velocity.
		/// </summary>
		/// <param name="quantity">Quantity.</param>
		/// <param name="givePosition">Position.</param>
		/// <param name="randomVelocityMin">Random velocity minimum.</param>
		/// <param name="randomVelocityMax">Random velocity max.</param>
		/// <param name="giveColor">Color.</param>
		public void Emit (int quantity, Vector3 givePosition, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor) {
			source = SOURCEC.Script;
			for (int i = 0; i<quantity; i++)
				EmitProcedure(
					givePosition,
					applyInitialVelocityShape?
					Vector3.Scale (RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((i*1f)/(quantity*1f), initialVelocityShapeScale))
					:
					RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax),
					giveColor
				);
			particleSystemGameObject.SetActive(true);
		}

		/// <summary>
		/// Emits number of particles set by quantity, minimum - maximum random position and velocity.
		/// </summary>
		/// <param name="quantity">Quantity.</param>
		/// <param name="randomPositionMin">Random position minimum.</param>
		/// <param name="randomPositionMax">Random position max.</param>
		/// <param name="randomVelocityMin">Random velocity minimum.</param>
		/// <param name="randomVelocityMax">Random velocity max.</param>
		/// <param name="giveColor">Color.</param>
		public void Emit (int quantity, Vector3 randomPositionMin, Vector3 randomPositionMax, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor) {
			source = SOURCEC.Script;
			for (int i = 0; i<quantity; i++)
				EmitProcedure(
					RandomRange(internalRandom01, randomPositionMin, randomPositionMax),
					applyInitialVelocityShape?
						Vector3.Scale (RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((i*1f)/(quantity*1f), initialVelocityShapeScale))
					:
						RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax),
					giveColor
				);
			particleSystemGameObject.SetActive(true);
		}

		/// <summary>
		/// Thread-safe version of Emit().
		/// </summary>
		/// <param name="givePosition">Position.</param>
		/// <param name="giveVelocity">Velocity.</param>
		/// <param name="giveColor">Color.</param>
		public void ThreadSafeEmit (Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor) {
			EmitProcedure(givePosition, giveVelocity, giveColor);
		}
		public void ThreadSafeEmit (int quantity, Vector3 givePosition, Vector3 randomVelocityMin, Vector3 randomVelocityMax, Color32 giveColor) {
			for (int i = 0; i<quantity; i++)
				EmitProcedure(
					givePosition,
					applyInitialVelocityShape?
					Vector3.Scale (RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax), initialVelocityShape.Evaluate((i*1f)/(quantity*1f), initialVelocityShapeScale))
					:
					RandomRange(internalRandom01, randomVelocityMin, randomVelocityMax),
					giveColor
				);
		}

		/// <summary>
		/// Internal emission procedure called upon Emit().
		/// </summary>
		/// <param name="givePosition">Give position.</param>
		/// <param name="giveVelocity">Give velocity.</param>
		/// <param name="giveColor">Give color.</param>
		void EmitProcedure (Vector3 givePosition, Vector3 giveVelocity, Color32 giveColor) {
			scriptedEmissionIndex=scriptedEmissionIndex%particleCount;
			scriptedEmissionPosition = givePosition;
			scriptedEmissionVelocity = giveVelocity;
			scriptedEmissionColor = giveColor;
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;
			cameFromNonCalculatedFrame = false;
			cameFromNonEmissionFrame = false;

			playgroundCache.simulate[scriptedEmissionIndex] = true;

			Rebirth(thisInstance, scriptedEmissionIndex, internalRandom01);

			if (playgroundCache.lifetimeOffset.Length!=particleCount) return;

			playgroundCache.initialColor[scriptedEmissionIndex] = scriptedEmissionColor;
			playgroundCache.lifetimeOffset[scriptedEmissionIndex] = 0;
			playgroundCache.life[scriptedEmissionIndex] = 0;
			playgroundCache.birth[scriptedEmissionIndex] = PlaygroundC.globalTime;
			playgroundCache.death[scriptedEmissionIndex] = playgroundCache.birth[scriptedEmissionIndex]+lifetime;
			playgroundCache.emission[scriptedEmissionIndex] = true;
			playgroundCache.scriptedColor[scriptedEmissionIndex] = new Color(giveColor.r, giveColor.g, giveColor.b, giveColor.a);
			playgroundCache.isFirstLoop[scriptedEmissionIndex] = true;

			emit = true;
			previousEmission = true;
			simulationStarted = PlaygroundC.globalTime;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			scriptedEmissionIndex++;scriptedEmissionIndex=scriptedEmissionIndex%particleCount;
		}
		
		/// <summary>
		/// Checks if particles are still in simulation.
		/// </summary>
		/// <returns><c>true</c> if this particle system is alive; otherwise, <c>false</c>.</returns>
		public bool IsAlive () {
			return hasActiveParticles;
		}

		/// <summary>
		/// Determines whether this particle system is simulated in local space.
		/// </summary>
		/// <returns><c>true</c> if this particle system is simulated in local space; otherwise, <c>false</c>.</returns>
		public bool IsLocalSpace () {
			return localSpace;
		}

		/// <summary>
		/// Determines if this particle system is in a transition.
		/// </summary>
		/// <returns><c>true</c>, if transition is active, <c>false</c> otherwise.</returns>
		public bool InTransition () {
			return inTransition;
		}

		/// <summary>
		/// Determines whether this particle system is loading a snapshot.
		/// </summary>
		/// <returns><c>true</c> if this particle system is loading a snapshot; otherwise, <c>false</c>.</returns>
		public bool IsLoading () {
			return isLoading;
		}

		/// <summary>
		/// Determines whether this particle system is yield refreshing.
		/// </summary>
		/// <returns><c>true</c> if this particle system is yield refreshing; otherwise, <c>false</c>.</returns>
		public bool IsYieldRefreshing () {
			return isYieldRefreshing;
		}

		/// <summary>
		/// Determines whether the skinned world object is ready.
		/// </summary>
		/// <returns><c>true</c> if this skinned world object is ready; otherwise, <c>false</c>.</returns>
		public bool IsSkinnedWorldObjectReady () {
			return source==SOURCEC.SkinnedWorldObject&&skinnedWorldObjectReady;
		}

		/// <summary>
		/// Determines whether this particle system has turbulence active.
		/// </summary>
		/// <returns><c>true</c> if this particle system has turbulence; otherwise, <c>false</c>.</returns>
		public bool HasTurbulence () {
			return !onlySourcePositioning && !onlyLifetimePositioning && turbulenceStrength>0 && turbulenceType!=TURBULENCETYPE.None;
		}

		/// <summary>
		/// Gets the current delta time of a particle system's update loop.
		/// </summary>
		/// <returns>The delta time.</returns>
		public float GetDeltaTime () {
			return t;
		}

		/// <summary>
		/// Gets the running simplex algorithm.
		/// </summary>
		/// <returns>The running simplex algorithm.</returns>
		public SimplexNoise GetSimplex () {
			return turbulenceSimplex;
		}

		/// <summary>
		/// Kill the specified particle.
		/// </summary>
		/// <param name="p">Index of particle.</param>
		public void Kill (int p) {
			playgroundCache.changedByPropertyDeath[p] = true;
			playgroundCache.life[p] = lifetime;
		}
		
		/// <summary>
		/// Sets a particle to no longer respond to forces.
		/// </summary>
		/// <param name="p">Index of particle.</param>
		/// <param name="noForce">If set to <c>true</c> then disable force for the particle at index.</param>
		public void SetNoForce (int p, bool noForce) {
			playgroundCache.noForce[p] = noForce;
		}

		/// <summary>
		/// Determines if the particle have noForce set to true
		/// </summary>
		/// <returns><c>true</c>, if force is disabled, <c>false</c> otherwise.</returns>
		/// <param name="p">Index of particle.</param>
		public bool NoForce (int p) {
			return playgroundCache.noForce[p];
		}

		/// <summary>
		/// Translate the specified particle
		/// </summary>
		/// <param name="p">Particle index.</param>
		/// <param name="translation">Translation.</param>
		public void Translate (int p, Vector3 translation) {
			playgroundCache.position[p] += translation;
		}

		/// <summary>
		/// Positions the specified particle.
		/// </summary>
		/// <param name="p">Particle index.</param>
		/// <param name="position">Position.</param>
		public void Position (int p, Vector3 position) {
			playgroundCache.position[p] = position;
		}

		/// <summary>
		/// Positions to transform point.
		/// </summary>
		/// <param name="p">Particle index.</param>
		/// <param name="position">Position.</param>
		/// <param name="targetTransform">Target transform.</param>
		public void PositionToTransformPoint (int p, Vector3 position, Transform targetTransform) {
			playgroundCache.position[p] = targetTransform.TransformPoint (position);
		}

		/// <summary>
		/// Positions to inverse transform point.
		/// </summary>
		/// <param name="p">Particle index.</param>
		/// <param name="position">Position.</param>
		/// <param name="targetTransform">Target transform.</param>
		public void PositionToInverseTransformPoint (int p, Vector3 position, Transform targetTransform) {
			playgroundCache.position[p] = targetTransform.InverseTransformPoint (position);
		}

		/// <summary>
		/// Gets the particle position.
		/// </summary>
		/// <returns>The particle position.</returns>
		/// <param name="p">Particle index.</param>
		public Vector3 GetParticlePosition (int p) {
			return playgroundCache.position[p];
		}

		public void SetHasActiveParticles () {
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;

		}

		/// <summary>
		/// Determines whether this particle system has several manipulator events.
		/// </summary>
		/// <returns><c>true</c> if this particle system has several manipulator events; otherwise, <c>false</c>.</returns>
		public bool HasSeveralManipulatorEvents () {
			return hasSeveralManipulatorEvents;
		}

		/// <summary>
		/// Protects the particle from specified manipulator.
		/// </summary>
		/// <param name="particle">Particle index.</param>
		/// <param name="manipulator">Manipulator.</param>
		public void ProtectParticleFromManipulator (int particle, ManipulatorObjectC manipulator) {
			if (manipulator.transform.Update())
				playgroundCache.excludeFromManipulatorId[particle] = manipulator.transform.instanceID;
		}

		/// <summary>
		/// Removes the particle protection.
		/// </summary>
		/// <param name="particle">Particle index.</param>
		public void RemoveParticleProtection (int particle) {
			playgroundCache.excludeFromManipulatorId[particle] = 0;
		}

		/// <summary>
		/// Creates a new PlaygroundParticlesC object.
		/// </summary>
		/// <returns>The playground particles.</returns>
		/// <param name="images">Images.</param>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="particleSize">Particle size.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC CreatePlaygroundParticles (Texture2D[] images, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material) {
			PlaygroundParticlesC playgroundParticles = CreateParticleObject(name,position,rotation,particleSize,material);
			
			int[] quantityList = new int[images.Length];
			int i = 0;
			for (; i<images.Length; i++)
				quantityList[i] = images[i].width*images[i].height;
			playgroundParticles.particleCache = new ParticleSystem.Particle[quantityList[PlaygroundC.Largest(quantityList)]];
			OnCreatePlaygroundParticles(playgroundParticles);	
			
			for (i = 0; i<images.Length; i++) {
				playgroundParticles.states.Add(new ParticleStateC());
				playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(images[i],scale,offset,"State 0",null);
			}
			
			return playgroundParticles;
		}
		
		/// <summary>
		/// Sets default settings for a PlaygroundParticlesC object.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void OnCreatePlaygroundParticles (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.playgroundCache = new PlaygroundCache();
			playgroundParticles.paint = new PaintObjectC();
			playgroundParticles.states = new List<ParticleStateC>();
			playgroundParticles.projection = new ParticleProjectionC();
			playgroundParticles.colliders = new List<PlaygroundColliderC>();
			playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity;
			playgroundParticles.projection.projectionTransform = playgroundParticles.particleSystemTransform;
			
			playgroundParticles.playgroundCache.initialSize = new float[playgroundParticles.particleCount];
			playgroundParticles.playgroundCache.initialSize = RandomFloat(playgroundParticles.playgroundCache.initialSize.Length, playgroundParticles.sizeMin, playgroundParticles.sizeMax, playgroundParticles.internalRandom01);
			
			playgroundParticles.previousParticleCount = playgroundParticles.particleCount;
			playgroundParticles.lifetimeSize = new AnimationCurve(new Keyframe(0,1), new Keyframe(1,1));
			
			playgroundParticles.shurikenParticleSystem.Emit(playgroundParticles.particleCount);
			playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache);
			for (int p = 0; p<playgroundParticles.particleCache.Length; p++) {
				playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p];
			}
			
			PlaygroundParticlesC.SetParticleCount(playgroundParticles, playgroundParticles.particleCount);
			
			if (PlaygroundC.reference!=null) {
				PlaygroundC.particlesQuantity++;
				PlaygroundC.reference.particleSystems.Add(playgroundParticles);
				playgroundParticles.particleSystemId = PlaygroundC.particlesQuantity;
			}
		}
		
		/// <summary>
		/// Creates a Shuriken Particle System.
		/// </summary>
		/// <returns>The particle object.</returns>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="particleSize">Particle size.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC CreateParticleObject (string name, Vector3 position, Quaternion rotation, float particleSize, Material material) {
			GameObject go = PlaygroundC.ResourceInstantiate("Particle Playground System");
			PlaygroundParticlesC playgroundParticles = go.GetComponent<PlaygroundParticlesC>();
			playgroundParticles.particleSystemGameObject = go;
			playgroundParticles.particleSystemGameObject.name = name;
			playgroundParticles.shurikenParticleSystem = playgroundParticles.particleSystemGameObject.GetComponent<ParticleSystem>();
			playgroundParticles.particleSystemRenderer = playgroundParticles.shurikenParticleSystem.renderer;
			playgroundParticles.particleSystemRenderer2 = playgroundParticles.shurikenParticleSystem.renderer as ParticleSystemRenderer;
			playgroundParticles.particleSystemTransform = playgroundParticles.particleSystemGameObject.transform;
			playgroundParticles.sourceTransform = playgroundParticles.particleSystemTransform;
			playgroundParticles.source = SOURCEC.Transform;
			playgroundParticles.particleSystemTransform.position = position;
			playgroundParticles.particleSystemTransform.rotation = rotation;

			if (PlaygroundC.reference.autoGroup && playgroundParticles.particleSystemTransform.parent==null)
				playgroundParticles.particleSystemTransform.parent = PlaygroundC.referenceTransform;
			
			if (playgroundParticles.particleSystemRenderer.sharedMaterial==null)
				playgroundParticles.particleSystemRenderer.sharedMaterial = material;

			return playgroundParticles;
		}
		
		/// <summary>
		/// Creates a new WorldObject.
		/// </summary>
		/// <returns>The world object.</returns>
		/// <param name="meshTransform">Mesh transform.</param>
		public static WorldObject NewWorldObject (Transform meshTransform) {
			WorldObject worldObject = new WorldObject();
			if (meshTransform.GetComponentInChildren<MeshFilter>()) {
				worldObject.transform = meshTransform;
				worldObject.Initialize ();
			} else Debug.Log("Could not find a mesh in "+meshTransform.name+".");
			return worldObject;
		}
		
		/// <summary>
		/// Creates a new SkinnedWorldObject.
		/// </summary>
		/// <returns>The skinned world object.</returns>
		/// <param name="meshTransform">Mesh transform.</param>
		public static SkinnedWorldObject NewSkinnedWorldObject (Transform meshTransform) {
			SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();
			if (meshTransform.GetComponentInChildren<SkinnedMeshRenderer>()) {
				skinnedWorldObject.transform = meshTransform;
				skinnedWorldObject.Initialize ();
			} else Debug.Log("Could not find a skinned mesh in "+meshTransform.name+".");
			return skinnedWorldObject;
		}

		/// <summary>
		/// Creates a new SkinnedWorldObject with pre-set down resolution.
		/// </summary>
		/// <returns>The skinned world object.</returns>
		/// <param name="meshTransform">Mesh transform.</param>
		/// <param name="downResolution">Down resolution.</param>
		public static SkinnedWorldObject NewSkinnedWorldObject (Transform meshTransform, int downResolution) {
			SkinnedWorldObject skinnedWorldObject = NewSkinnedWorldObject(meshTransform);
			skinnedWorldObject.downResolution = downResolution;
			return skinnedWorldObject;
		}
		
		/// <summary>
		/// Creates a new PaintObject.
		/// </summary>
		/// <returns>The paint object.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static PaintObjectC NewPaintObject (PlaygroundParticlesC playgroundParticles) {
			PaintObjectC paintObject = new PaintObjectC();
			playgroundParticles.paint = paintObject;
			playgroundParticles.paint.Initialize();
			return paintObject;
		}
		
		/// <summary>
		/// Creates a new ParticleProjection object.
		/// </summary>
		/// <returns>The projection object.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ParticleProjectionC NewProjectionObject (PlaygroundParticlesC playgroundParticles) {
			ParticleProjectionC projectionObject = new ParticleProjectionC();
			playgroundParticles.projection = projectionObject;
			playgroundParticles.projection.Initialize();
			return projectionObject;
		}
		
		/// <summary>
		/// Creates a new ManipulatorObject and attach to the Playground Manager.
		/// </summary>
		/// <returns>The manipulator object.</returns>
		/// <param name="type">Type.</param>
		/// <param name="affects">Affects.</param>
		/// <param name="manipulatorTransform">Manipulator transform.</param>
		/// <param name="size">Size.</param>
		/// <param name="strength">Strength.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ManipulatorObjectC NewManipulatorObject (MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength, PlaygroundParticlesC playgroundParticles) {
			ManipulatorObjectC manipulatorObject = new ManipulatorObjectC();
			manipulatorObject.type = type;
			manipulatorObject.affects = affects;
			manipulatorObject.transform.transform = manipulatorTransform;
			manipulatorObject.size = size;
			manipulatorObject.strength = strength;
			manipulatorObject.bounds = new Bounds(Vector3.zero, new Vector3(size, size, size));
			manipulatorObject.property = new ManipulatorPropertyC();
			manipulatorObject.Update();

			// Add this to Playground Manager or the passed in playgroundParticles
			if (playgroundParticles==null)
				PlaygroundC.reference.manipulators.Add(manipulatorObject);
			else
				playgroundParticles.manipulators.Add(manipulatorObject);
			
			return manipulatorObject;
		}

		// Set color from PixelParticle object
		public static void SetColor (PlaygroundParticlesC playgroundParticles, int to) {
			Color color = new Color();
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				color = playgroundParticles.states[to].GetColor(i%playgroundParticles.states[to].colorLength);
				playgroundParticles.particleCache[i].color = color;
			}
		}
		
		// Set color from Color
		public static void SetColor (PlaygroundParticlesC playgroundParticles, Color color) {
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				playgroundParticles.particleCache[i].color = color;
			}
		}

		// Get vertices from a skinned world object in a Vector3-array
		public static void GetPosition (SkinnedWorldObject particleStateWorldObject, bool updateNormals) {
			if (updateNormals)
				particleStateWorldObject.normals = particleStateWorldObject.mesh.normals;
			Vector3[] vertices = particleStateWorldObject.mesh.vertices;
			BoneWeight[] weights = particleStateWorldObject.mesh.boneWeights;
			Matrix4x4[] bindPoses = particleStateWorldObject.mesh.bindposes;
			Matrix4x4[] boneMatrices = new Matrix4x4[particleStateWorldObject.renderer.bones.Length];

			int i = 0;
			for (; i<boneMatrices.Length; i++) {
				boneMatrices[i] = particleStateWorldObject.renderer.bones[i].localToWorldMatrix * bindPoses[i];
			}

			PlaygroundC.RunAsync(()=>{
				Matrix4x4 vertexMatrix = new Matrix4x4();
				for (i = 0; i<vertices.Length; i++) {
					BoneWeight weight = weights[i];
					Matrix4x4 m0 = boneMatrices[weight.boneIndex0];
					Matrix4x4 m1 = boneMatrices[weight.boneIndex1];
					Matrix4x4 m2 = boneMatrices[weight.boneIndex2];
					Matrix4x4 m3 = boneMatrices[weight.boneIndex3];
					
					for (int n=0;n<16;n++) {
						vertexMatrix[n] =
							m0[n] * weight.weight0 +
							m1[n] * weight.weight1 +
							m2[n] * weight.weight2 +
							m3[n] * weight.weight3;
					}
					vertices[i] = vertexMatrix.MultiplyPoint3x4(vertices[i]);
				}
				particleStateWorldObject.vertexPositions = vertices;
			});
		}
		
		// Get position from Mesh World Object
		public static void GetPosition (Vector3[] v3, WorldObject particleStateWorldObject) {
			if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
				particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
			v3 = particleStateWorldObject.mesh.vertices;
		}

		// Get procedural position from Mesh World Object
		public static void GetProceduralPosition (Vector3[] v3, WorldObject particleStateWorldObject) {
			if (particleStateWorldObject.meshFilter.sharedMesh!=particleStateWorldObject.mesh)
				particleStateWorldObject.mesh = particleStateWorldObject.meshFilter.sharedMesh;
			Vector3[] vertices = particleStateWorldObject.mesh.vertices;
			if (v3.Length!=vertices.Length) v3 = new Vector3[vertices.Length];
			for (int i = 0; i<v3.Length; i++) {
				v3[i] = particleStateWorldObject.transform.TransformPoint(vertices[i%vertices.Length]);
			}
		}
		
		// Get normals from Mesh World Object
		public static void GetNormals (Vector3[] v3, WorldObject particleStateWorldObject) {
			v3 = particleStateWorldObject.mesh.normals;
		}
		
		// Set size for particles
		public static void SetSize (PlaygroundParticlesC playgroundParticles, float size) {
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				playgroundParticles.playgroundCache.initialSize[i] = size;
				playgroundParticles.particleCache[i].size = size;
			}
		}

		public void RefreshSystemRandom () {
			internalRandom01 = new System.Random();
		}
		
		// Set random size for particles within sizeMinimum- and sizeMaximum range 
		public static void SetSizeRandom (PlaygroundParticlesC playgroundParticles, float sizeMinimum, float sizeMaximum) {
			playgroundParticles.playgroundCache.initialSize = RandomFloat(playgroundParticles.particleCache.Length, sizeMinimum, sizeMaximum, playgroundParticles.internalRandom01);
			playgroundParticles.sizeMin = sizeMinimum;
			playgroundParticles.sizeMax = sizeMaximum;
			playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
			playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
		}
		
		// Set random rotation for particles within rotationMinimum- and rotationMaximum range
		public static void SetRotationRandom (PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum) {
			playgroundParticles.playgroundCache.rotationSpeed = RandomFloat(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				playgroundParticles.playgroundCache.rotation[i] = playgroundParticles.playgroundCache.initialRotation[i];
			}
			playgroundParticles.rotationSpeedMin = rotationMinimum;
			playgroundParticles.rotationSpeedMax = rotationMaximum;
			playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
			playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
		}
		
		// Set random initial rotation for particles within rotationMinimum- and rotationMaximum range
		public static void SetInitialRotationRandom (PlaygroundParticlesC playgroundParticles, float rotationMinimum, float rotationMaximum) {
			playgroundParticles.playgroundCache.initialRotation = RandomFloat(playgroundParticles.particleCache.Length, rotationMinimum, rotationMaximum, playgroundParticles.internalRandom01);
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				playgroundParticles.playgroundCache.rotation[i] = playgroundParticles.playgroundCache.initialRotation[i];
			}
			playgroundParticles.initialRotationMin = rotationMinimum;
			playgroundParticles.initialRotationMax = rotationMaximum;
			playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
			playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
		}
		
		// Set initial random velocity for particles within velocityMinimum- and velocityMaximum range
		public static void SetVelocityRandom (PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum) {
			playgroundParticles.playgroundCache.initialVelocity = new Vector3[playgroundParticles.particleCount];
			for (int i = 0; i<playgroundParticles.particleCount; i++) {
				playgroundParticles.playgroundCache.initialVelocity[i] = new Vector3(
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x),
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.y, velocityMaximum.y),
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.z, velocityMaximum.z)
					);
			}
			
			playgroundParticles.initialVelocityMin = velocityMinimum;
			playgroundParticles.initialVelocityMax = velocityMaximum;
			playgroundParticles.previousVelocityMin = playgroundParticles.initialVelocityMin;
			playgroundParticles.previousVelocityMax = playgroundParticles.initialVelocityMax;
		}
		
		// Set initial random local velocity for particles within velocityMinimum- and velocityMaximum range
		public static void SetLocalVelocityRandom (PlaygroundParticlesC playgroundParticles, Vector3 velocityMinimum, Vector3 velocityMaximum) {
			playgroundParticles.playgroundCache.initialLocalVelocity = new Vector3[playgroundParticles.particleCount];
			for (int i = 0; i<playgroundParticles.particleCount; i++) {
				playgroundParticles.playgroundCache.initialLocalVelocity[i] = new Vector3(
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.x, velocityMaximum.x),
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.y, velocityMaximum.y),
					RandomRange(playgroundParticles.internalRandom01, velocityMinimum.z, velocityMaximum.z)
				);
			}

			playgroundParticles.initialLocalVelocityMin = velocityMinimum;
			playgroundParticles.initialLocalVelocityMax = velocityMaximum;
			playgroundParticles.previousLocalVelocityMin = playgroundParticles.initialLocalVelocityMin;
			playgroundParticles.previousLocalVelocityMax = playgroundParticles.initialLocalVelocityMax;
		}

		// Set material for particle system
		public static void SetMaterial (PlaygroundParticlesC playgroundParticles, Material particleMaterial) {
			playgroundParticles.particleSystemRenderer.sharedMaterial = particleMaterial;
		}
		
		// Set alphas for particles
		public static void SetAlpha (PlaygroundParticlesC playgroundParticles, float alpha) {
			Color pColor;
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++) {
				pColor = playgroundParticles.particleCache[i].color;
				pColor.a = alpha;
				playgroundParticles.particleCache[i].color = pColor;
			}
		}
		

		
		// Move all particles in direction
		public static void Translate (PlaygroundParticlesC playgroundParticles, Vector3 direction) {
			for (int i = 0; i<playgroundParticles.particleCache.Length; i++)
				playgroundParticles.particleCache[i].position += direction;
		}
		
		// Add new state from state
		public static void Add (PlaygroundParticlesC playgroundParticles, ParticleStateC state) {
			playgroundParticles.states.Add(state);
			state.Initialize();
		}
		
		// Add new state from image
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			playgroundParticles.states.Add(new ParticleStateC());
			playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
		}
		
		// Add new state from image with depthmap
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			playgroundParticles.states.Add(new ParticleStateC());
			playgroundParticles.states[playgroundParticles.states.Count-1].ConstructParticles(image,scale,offset,stateName,stateTransform);
			playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmap = depthmap;
			playgroundParticles.states[playgroundParticles.states.Count-1].stateDepthmapStrength = depthmapStrength;
		}
		
		// Destroy a PlaygroundParticlesC object
		public static void Destroy (PlaygroundParticlesC playgroundParticles) {
			Clear(playgroundParticles);
			MonoBehaviour.DestroyImmediate(playgroundParticles.particleSystemGameObject);
			playgroundParticles = null;
		}

		public bool IsSettingLifetime () {
			return isSettingLifetime;
		}

		// Sorts the particles in lifetime
		bool isSettingLifetime = false;
		public static void SetLifetime (PlaygroundParticlesC playgroundParticles, SORTINGC sorting, float time) {

			if (!playgroundParticles.enabled) return;
			if (playgroundParticles.isSettingLifetime || playgroundParticles.isSettingParticleCount) return;

			if (playgroundParticles.internalRandom01==null)
				playgroundParticles.RefreshSystemRandom();

			playgroundParticles.isSettingLifetime = true;
			PlaygroundC.RunAsync(()=>{
				playgroundParticles.lifetime = time;
				SetLifetimeSubtraction(playgroundParticles);
				playgroundParticles.playgroundCache.lifetimeOffset = new float[playgroundParticles.particleCount];
				int pCount = playgroundParticles.playgroundCache.lifetimeOffset.Length;
				if (playgroundParticles.source!=SOURCEC.Script) {
					switch (sorting) {
					case SORTINGC.Scrambled:
						for (int r = 0; r<playgroundParticles.particleCount; r++) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							playgroundParticles.playgroundCache.lifetimeOffset[r] = RandomRange(playgroundParticles.internalRandom01, 0f, playgroundParticles.lifetime);
						}
					break;
					case SORTINGC.ScrambledLinear:
						float slPerc;
						for (int sl = 0; sl<playgroundParticles.particleCount; sl++) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							slPerc = (sl*1f)/(playgroundParticles.particleCount*1f);
							playgroundParticles.playgroundCache.lifetimeOffset[sl] = playgroundParticles.lifetime*slPerc;
						}
						for (int i = playgroundParticles.playgroundCache.lifetimeOffset.Length-1; i>0; i--) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							int r = playgroundParticles.internalRandom01.Next(0,i);
							float tmp = playgroundParticles.playgroundCache.lifetimeOffset[i];
							playgroundParticles.playgroundCache.lifetimeOffset[i] = playgroundParticles.playgroundCache.lifetimeOffset[r];
							playgroundParticles.playgroundCache.lifetimeOffset[r] = tmp;
						}
					break;
					case SORTINGC.Burst:
						// No action needed for spawning all particles at once
					break;
					case SORTINGC.Reversed:
						float lPerc;
						for (int l = 0; l<playgroundParticles.particleCount; l++) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							lPerc = (l*1f)/(playgroundParticles.particleCount*1f);
							playgroundParticles.playgroundCache.lifetimeOffset[l] = playgroundParticles.lifetime*lPerc;
						}
					break;
					case SORTINGC.Linear:
						float rPerc;
						int rInc = 0;
						for (int r = playgroundParticles.particleCount-1; r>=0; r--) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							rPerc = (rInc*1f)/(playgroundParticles.particleCount*1f);
							rInc++;
							playgroundParticles.playgroundCache.lifetimeOffset[r] = playgroundParticles.lifetime*rPerc;
						}
					break;
					case SORTINGC.NearestNeighbor:
						playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
						float[] nnDist = new float[playgroundParticles.particleCount];
						float nnHighest = 0;
						int nn = 0;
						for (; nn<playgroundParticles.particleCount; nn++) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							nnDist[nn%playgroundParticles.particleCount] = Vector3.Distance(playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin%playgroundParticles.particleCount], playgroundParticles.playgroundCache.targetPosition[nn%playgroundParticles.particleCount]);
							if (nnDist[nn%playgroundParticles.particleCount]>nnHighest)
								nnHighest = nnDist[nn%playgroundParticles.particleCount];
						}
						if (nnHighest>0) {
							for (nn = 0; nn<playgroundParticles.particleCount; nn++) {
								if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
								playgroundParticles.playgroundCache.lifetimeOffset[nn%playgroundParticles.particleCount] = Mathf.Lerp(playgroundParticles.lifetime, 0, (nnDist[nn%playgroundParticles.particleCount]/nnHighest));
							}
						} else {
							for (nn = 0; nn<playgroundParticles.particleCount; nn++) {
								if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
								playgroundParticles.playgroundCache.lifetimeOffset[nn%playgroundParticles.particleCount] = 0;
							}
						}
					break;
					case SORTINGC.NearestNeighborReversed:
						playgroundParticles.nearestNeighborOrigin = Mathf.Clamp(playgroundParticles.nearestNeighborOrigin, 0, playgroundParticles.particleCount-1);
						float[] nnrDist = new float[playgroundParticles.particleCount];
						float nnrHighest = 0;
						int nnr = 0;
						for (; nnr<playgroundParticles.particleCount; nnr++) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							nnrDist[nnr%playgroundParticles.particleCount] = Vector3.Distance(playgroundParticles.playgroundCache.targetPosition[playgroundParticles.nearestNeighborOrigin], playgroundParticles.playgroundCache.targetPosition[nnr%playgroundParticles.particleCount]);
							if (nnrDist[nnr%playgroundParticles.particleCount]>nnrHighest)
								nnrHighest = nnrDist[nnr%playgroundParticles.particleCount];
						}
						if (nnrHighest>0) {
							for (nnr = 0; nnr<playgroundParticles.particleCount; nnr++) {
								if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
								playgroundParticles.playgroundCache.lifetimeOffset[nnr%playgroundParticles.particleCount] = Mathf.Lerp(0, playgroundParticles.lifetime, (nnrDist[nnr%playgroundParticles.particleCount]/nnrHighest));
							}
						} else {
							for (nnr = 0; nnr<playgroundParticles.particleCount; nnr++) {
								if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
								playgroundParticles.playgroundCache.lifetimeOffset[nnr%playgroundParticles.particleCount] = 0;
							}
						}
					break;
					case SORTINGC.Custom:
						for (int cs = playgroundParticles.particleCount-1; cs>=0; cs--) {
							if (pCount!=playgroundParticles.playgroundCache.lifetimeOffset.Length) {playgroundParticles.isSettingLifetime = false; return;}
							playgroundParticles.playgroundCache.lifetimeOffset[cs] = playgroundParticles.lifetime*playgroundParticles.lifetimeSorting.Evaluate(cs*1f/playgroundParticles.particleCount*1f);
						}
					break;
					}
				}

				SetEmissionRate(playgroundParticles);
				SetParticleTimeNow(playgroundParticles);
				playgroundParticles.previousLifetime = playgroundParticles.lifetime;
				playgroundParticles.previousNearestNeighborOrigin = playgroundParticles.nearestNeighborOrigin;
				playgroundParticles.previousSorting = playgroundParticles.sorting;
				playgroundParticles.isDoneThread = true;
				playgroundParticles.isSettingLifetime = false;
				playgroundParticles.lastTimeUpdated = PlaygroundC.globalTime;
			});
		}

		public static void SetLifetimeSubtraction (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.playgroundCache.lifetimeSubtraction = new float[playgroundParticles.particleCount];
			System.Random random = new System.Random();
			for (int p = 0; p<playgroundParticles.playgroundCache.lifetimeSubtraction.Length; p++) {
				if (playgroundParticles.lifetimeValueMethod==VALUEMETHOD.Constant) {
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = 0;
				} else {
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = RandomRange (random, 0, playgroundParticles.lifetime-playgroundParticles.lifetimeMin);
				}
			}
			playgroundParticles.previousLifetimeMin = playgroundParticles.lifetimeMin;
			playgroundParticles.previousLifetimeValueMethod = playgroundParticles.lifetimeValueMethod;
		}
		
		// Set emission rate percentage of particle count
		public static void SetEmissionRate (PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.playgroundCache.emission==null) return;
			float rateCount = playgroundParticles.lifetime*playgroundParticles.emissionRate;
			int currentCount = playgroundParticles.playgroundCache.emission.Length;
			bool hasOverflow = playgroundParticles.source!=SOURCEC.Transform&&(playgroundParticles.overflowOffset!=Vector3.zero||playgroundParticles.applySourceScatter&&(playgroundParticles.sourceScatterMin!=Vector3.zero||playgroundParticles.sourceScatterMax!=Vector3.zero));
			for (int p = 0; p<playgroundParticles.playgroundCache.emission.Length; p++) {
				if (currentCount!=playgroundParticles.playgroundCache.emission.Length || playgroundParticles.playgroundCache.lifetimeOffset.Length!=currentCount) return;
				if (playgroundParticles.emissionRate!=0 && playgroundParticles.source!=SOURCEC.Script) {
					if (playgroundParticles.sorting!=SORTINGC.Burst || playgroundParticles.sorting==SORTINGC.NearestNeighbor && hasOverflow || playgroundParticles.sorting==SORTINGC.NearestNeighborReversed && hasOverflow) {
						playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.playgroundCache.lifetimeOffset[p]>=playgroundParticles.lifetime-rateCount && playgroundParticles.emit);
					} else {
						playgroundParticles.playgroundCache.emission[p] = (playgroundParticles.emit && playgroundParticles.emissionRate>((p*1f)/(currentCount*1f)));
					}
				} else playgroundParticles.playgroundCache.emission[p] = false;
				if (playgroundParticles.playgroundCache.emission[p]) {
					playgroundParticles.playgroundCache.rebirth[p] = true;
					playgroundParticles.playgroundCache.simulate[p] = true;
				} else if (playgroundParticles.source==SOURCEC.Script)
					playgroundParticles.playgroundCache.rebirth[p] = false;
			}
			playgroundParticles.previousEmissionRate = playgroundParticles.emissionRate;
			playgroundParticles.hasActiveParticles = true;
			playgroundParticles.threadHadNoActiveParticles = false;
		}
		
		// Set life and death of particles
		public static void SetParticleTimeNow (PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.playgroundCache.lifetimeOffset==null || playgroundParticles.playgroundCache.lifetimeOffset.Length!=playgroundParticles.particleCount) return;
			if (playgroundParticles.playgroundCache.life==null || playgroundParticles.playgroundCache.life.Length!=playgroundParticles.particleCount) return;
			if (playgroundParticles.source!=SOURCEC.Script) {
				float currentTime = PlaygroundC.globalTime+playgroundParticles.lifetimeOffset;
				int currentCount = playgroundParticles.particleCount;
				bool hasOverflow = playgroundParticles.source!=SOURCEC.Transform&&(playgroundParticles.overflowOffset!=Vector3.zero||playgroundParticles.applySourceScatter&&(playgroundParticles.sourceScatterMin!=Vector3.zero||playgroundParticles.sourceScatterMax!=Vector3.zero));
				playgroundParticles.simulationStarted = currentTime;

				if (playgroundParticles.sorting!=SORTINGC.Burst || playgroundParticles.sorting==SORTINGC.NearestNeighbor && hasOverflow || playgroundParticles.sorting==SORTINGC.NearestNeighborReversed && hasOverflow) {
					for (int p = 0; p<playgroundParticles.particleCount; p++) {
						if (currentCount!=playgroundParticles.particleCount) return;
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime-(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]+.01f);
						playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.playgroundCache.life[p];
						playgroundParticles.playgroundCache.death[p] = currentTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
						playgroundParticles.particleCache[p%playgroundParticles.particleCache.Length].startLifetime = playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.targetPosition[p] = PlaygroundC.initialTargetPosition;
						playgroundParticles.playgroundCache.previousTargetPosition[p] = PlaygroundC.initialTargetPosition;
						playgroundParticles.playgroundCache.position[p] = PlaygroundC.initialTargetPosition;
						playgroundParticles.playgroundCache.simulate[p] = true;
						playgroundParticles.playgroundCache.isFirstLoop[p] = true;
						playgroundParticles.playgroundCache.isNonBirthed[p] = true;
					}
				} else {
					currentTime = PlaygroundC.globalTime;
					// Force recalculation for burst mode to initiate sequence directly
					for (int p = 0; p<playgroundParticles.particleCount; p++) {
						if (currentCount!=playgroundParticles.particleCount) return;
						playgroundParticles.playgroundCache.lifetimeOffset[p] = 0;
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.death[p] = currentTime;
						playgroundParticles.particleCache[p%playgroundParticles.particleCache.Length].startLifetime = playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.simulate[p] = true;
						playgroundParticles.playgroundCache.isFirstLoop[p] = true;
						playgroundParticles.playgroundCache.isNonBirthed[p] = true;
					}

				}
			}
		}
		
		// Set life and death of particles after emit has changed
		public static void SetParticleTimeNowWithRestEmission (PlaygroundParticlesC playgroundParticles) {
			float currentGlobalTime = PlaygroundC.globalTime;
			float currentTime = currentGlobalTime+playgroundParticles.lifetimeOffset;
			float emissionDelta = currentGlobalTime-playgroundParticles.emissionStopped;
			bool applyDelta = false;
			bool hasOverflow = playgroundParticles.source!=SOURCEC.Transform&&(playgroundParticles.overflowOffset!=Vector3.zero||playgroundParticles.applySourceScatter&&(playgroundParticles.sourceScatterMin!=Vector3.zero||playgroundParticles.sourceScatterMax!=Vector3.zero));
			if (playgroundParticles.loop && emissionDelta<playgroundParticles.lifetime && emissionDelta>0) {
				applyDelta = true;
				playgroundParticles.cameFromNonEmissionFrame = true;
			}
			for (int p = 0; p<playgroundParticles.particleCount; p++) {
				if (!applyDelta) {
					if (playgroundParticles.sorting!=SORTINGC.Burst || playgroundParticles.sorting==SORTINGC.NearestNeighbor && hasOverflow || playgroundParticles.sorting==SORTINGC.NearestNeighborReversed && hasOverflow) {
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime-(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]+.01f);
						playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.playgroundCache.life[p];
						playgroundParticles.playgroundCache.death[p] = currentTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeOffset[p]);
						if (!playgroundParticles.loop) {
							playgroundParticles.InactivateParticle(p);
						}
					} else {
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.birth[p] = currentTime-playgroundParticles.lifetime;
						playgroundParticles.playgroundCache.death[p] = currentTime;
					}
					playgroundParticles.playgroundCache.birthDelay[p] = 0f;
					playgroundParticles.playgroundCache.isNonBirthed[p] = true;
					playgroundParticles.playgroundCache.isFirstLoop[p] = true;
					playgroundParticles.playgroundCache.rebirth[p] = true;
					playgroundParticles.playgroundCache.simulate[p] = true;
				} else {
					playgroundParticles.playgroundCache.birthDelay[p] = emissionDelta;
				}

				playgroundParticles.playgroundCache.emission[p] = playgroundParticles.emit;
			}
		}
		
		// Get color from evaluated lifetime color value where time is normalized
		public static Color32 GetColorAtLifetime (PlaygroundParticlesC playgroundParticles, float time) {
			return playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
		}
		
		// Color all particles from evaluated lifetime color value where time is normalized
		public static void SetColorAtLifetime (PlaygroundParticlesC playgroundParticles, float time) {
			Color32 c = playgroundParticles.lifetimeColor.Evaluate(time/playgroundParticles.lifetime);
			for (int p = 0; p<playgroundParticles.particleCount; p++)
				playgroundParticles.particleCache[p].color = c;
			playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
		}
		
		// Color all particles from lifetime span with sorting
		public static void SetColorWithLifetimeSorting (PlaygroundParticlesC playgroundParticles) {
			SetLifetime(playgroundParticles, playgroundParticles.sorting, playgroundParticles.lifetime);
			Color32 c;
			for (int p = 0; p<playgroundParticles.particleCount; p++) {
				c = playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime);
				playgroundParticles.particleCache[p].color = c;
			}
			playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
		}

		public bool IsSettingParticleCount () {
			return isSettingParticleCount;
		}

		// Sets the amount of particles and initiates the necessary arrays
		bool isSettingParticleCount = false;
		public static void SetParticleCount (PlaygroundParticlesC playgroundParticles, int amount) {
			if (playgroundParticles.isSettingParticleCount) return;
			if (amount<0) amount = 0;

			if (playgroundParticles.internalRandom01==null)
				playgroundParticles.RefreshSystemRandom();

			playgroundParticles.particleCount = amount;
			playgroundParticles.previousParticleCount = amount;

			// Create Particles
			playgroundParticles.particleCache = new ParticleSystem.Particle[amount];
			playgroundParticles.shurikenParticleSystem.Emit(amount);
			playgroundParticles.shurikenParticleSystem.GetParticles(playgroundParticles.particleCache);

			playgroundParticles.inTransition = false;
			playgroundParticles.hasActiveParticles = true;
			playgroundParticles.threadHadNoActiveParticles = false;

			playgroundParticles.isSettingParticleCount = true;
			PlaygroundC.RunAsync(()=>{
				if (!playgroundParticles.isSettingParticleCount) return;
				if (playgroundParticles.playgroundCache==null)
					playgroundParticles.playgroundCache = new PlaygroundCache();
				
				// Rebuild Cache
				playgroundParticles.playgroundCache.size = new float[amount];
				playgroundParticles.playgroundCache.birth = new float[amount];
				playgroundParticles.playgroundCache.death = new float[amount];
				playgroundParticles.playgroundCache.rebirth = new bool[amount];
				playgroundParticles.playgroundCache.birthDelay = new float[amount];
				playgroundParticles.playgroundCache.life = new float[amount];
				playgroundParticles.playgroundCache.lifetimeSubtraction = new float[amount];
				playgroundParticles.playgroundCache.rotation = new float[amount];
				playgroundParticles.playgroundCache.lifetimeOffset = new float[amount];
				playgroundParticles.playgroundCache.emission = new bool[amount];
				playgroundParticles.playgroundCache.changedByProperty = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertyColor = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertyColorLerp = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertySize = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertyTarget = new bool[amount];
				playgroundParticles.playgroundCache.changedByPropertyDeath = new bool[amount];
				playgroundParticles.playgroundCache.propertyTarget = new int[amount];
				playgroundParticles.playgroundCache.propertyId = new int[amount];
				playgroundParticles.playgroundCache.excludeFromManipulatorId = new int[amount];
				playgroundParticles.playgroundCache.propertyColorId = new int[amount];
				playgroundParticles.playgroundCache.manipulatorId = new int[amount];
				playgroundParticles.playgroundCache.color = new Color32[amount];
				playgroundParticles.playgroundCache.scriptedColor = new Color32[amount];
				playgroundParticles.playgroundCache.initialColor = new Color32[amount];
				playgroundParticles.playgroundCache.lifetimeColorId = new int[amount];
				playgroundParticles.playgroundCache.noForce = new bool[amount];
				playgroundParticles.playgroundCache.position = new Vector3[amount];
				playgroundParticles.playgroundCache.targetPosition = new Vector3[amount];
				playgroundParticles.playgroundCache.targetDirection = new Vector3[amount];
				playgroundParticles.playgroundCache.previousTargetPosition = new Vector3[amount];
				playgroundParticles.playgroundCache.previousParticlePosition = new Vector3[amount];
				playgroundParticles.playgroundCache.collisionParticlePosition = new Vector3[amount];
				playgroundParticles.playgroundCache.localSpaceMovementCompensation = new Vector3[amount];
				playgroundParticles.playgroundCache.scatterPosition = new Vector3[amount];
				playgroundParticles.playgroundCache.velocity = new Vector3[amount];
				playgroundParticles.playgroundCache.isMasked = new bool[amount];
				playgroundParticles.playgroundCache.maskAlpha = new float[amount];
				playgroundParticles.playgroundCache.isNonBirthed = new bool[amount];
				playgroundParticles.playgroundCache.isFirstLoop = new bool[amount];
				playgroundParticles.playgroundCache.simulate = new bool[amount];

				for (int i = 0; i<amount; i++) {
					playgroundParticles.particleCache[i].size = 0f;
					playgroundParticles.playgroundCache.rebirth[i] = true;
					playgroundParticles.playgroundCache.simulate[i] = true;
					playgroundParticles.playgroundCache.isNonBirthed[i] = true;
					playgroundParticles.playgroundCache.isFirstLoop[i] = true;
					
					// Set color gradient id
					if (playgroundParticles.colorSource==COLORSOURCEC.LifetimeColors && playgroundParticles.lifetimeColors.Count>0) {
						playgroundParticles.lifetimeColorId++;playgroundParticles.lifetimeColorId=playgroundParticles.lifetimeColorId%playgroundParticles.lifetimeColors.Count;
						playgroundParticles.playgroundCache.lifetimeColorId[i] = playgroundParticles.lifetimeColorId;
					}
				}
				
				// Set sizes
				SetSizeRandom(playgroundParticles, playgroundParticles.sizeMin, playgroundParticles.sizeMax);
				playgroundParticles.previousSizeMin = playgroundParticles.sizeMin;
				playgroundParticles.previousSizeMax = playgroundParticles.sizeMax;
				
				// Set rotations
				playgroundParticles.playgroundCache.initialRotation = RandomFloat(amount, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax, playgroundParticles.internalRandom01);
				playgroundParticles.playgroundCache.rotationSpeed = RandomFloat(amount, playgroundParticles.rotationSpeedMin, playgroundParticles.rotationSpeedMax, playgroundParticles.internalRandom01);
				playgroundParticles.previousInitialRotationMin = playgroundParticles.initialRotationMin;
				playgroundParticles.previousInitialRotationMax = playgroundParticles.initialRotationMax;
				playgroundParticles.previousRotationSpeedMin = playgroundParticles.rotationSpeedMin;
				playgroundParticles.previousRotationSpeedMax = playgroundParticles.rotationSpeedMax;
			
				// Set velocities
				SetVelocityRandom(playgroundParticles, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
				SetLocalVelocityRandom(playgroundParticles, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
				
				// Set Emission
				Emission(playgroundParticles, playgroundParticles.emit, false);
				
				// Make sure scatter is available first lifetime cycle
				if (playgroundParticles.applySourceScatter)
					playgroundParticles.RefreshScatter();
				playgroundParticles.isDoneThread = true;
				playgroundParticles.isSettingParticleCount = false;
			});
		}

		/// <summary>
		/// Updates a PlaygroundParticlesC object (called each calculation step from PlaygroundC).
		/// </summary>
		public bool UpdateSystem () {
			if (isYieldRefreshing || isLoading || playgroundCache==null || thisInstance==null || !enabled || isSettingLifetime || isSettingParticleCount) return false;

			// Nearest Neighbor sequence
			if ((sorting==SORTINGC.NearestNeighbor||sorting==SORTINGC.NearestNeighborReversed) && initNearestNeighborSeq<=5) {
				initNearestNeighborSeq++;
				if (initNearestNeighborSeq==2) {
					SetLifetime(thisInstance, SORTINGC.Burst, lifetime);
					return false;
				}
				if (initNearestNeighborSeq==5) {
					SetLifetime(thisInstance, sorting, lifetime);
					return false;
				}
			}

			// Emission halt for disabling called from calculation thread
			if (queueEmissionHalt) {
				if (disableOnDoneRoutine==ONDONE.Inactivate)
					particleSystemGameObject.SetActive(false);
				else if (Application.isPlaying) {
					DestroyImmediate(particleSystemGameObject);
					return false;
				}
			}

			// Particle count
			if (particleCount!=previousParticleCount) {
				SetParticleCount(thisInstance, particleCount);
				initNearestNeighborSeq = 0;
				isDoneThread = true;
				if (particleCount>0)
					StartCoroutine(Boot());
				return false;
			}

			// Particle emission
			if (emit!=previousEmission) {
				Emit(emit);
				return false;
			}

			// Particle loop enabled again
			if (loop!=previousLoop) {
				if (emit && loop)
					Emit(true);
				previousLoop = loop;
				return false;
			}

			// Particle size
			if (sizeMin!=previousSizeMin || sizeMax!=previousSizeMax)
				SetSizeRandom(thisInstance, sizeMin, sizeMax);
			
			// Particle rotation
			if (initialRotationMin!=previousInitialRotationMin || initialRotationMax!=previousInitialRotationMax)
				SetInitialRotationRandom(thisInstance, initialRotationMin, initialRotationMax);
			if (rotationSpeedMin!=previousRotationSpeedMin || rotationSpeedMax!=previousRotationSpeedMax)
				SetRotationRandom(thisInstance, rotationSpeedMin, rotationSpeedMax);
			
			// Particle velocity
			if (applyInitialVelocity) {
				if (initialVelocityMin!=previousVelocityMin || initialVelocityMax!=previousVelocityMax || playgroundCache.initialVelocity==null || playgroundCache.initialVelocity.Length!=particleCount) {
					SetVelocityRandom(thisInstance, initialVelocityMin, initialVelocityMax);
					return false;
				}
			}
			
			// Particle local velocity
			if (applyInitialLocalVelocity) {
				if (initialLocalVelocityMin!=previousLocalVelocityMin || initialLocalVelocityMax!=previousLocalVelocityMax || playgroundCache.initialLocalVelocity==null || playgroundCache.initialLocalVelocity.Length!=particleCount) {
					SetLocalVelocityRandom(thisInstance, initialLocalVelocityMin, initialLocalVelocityMax);
					return false;
				}
			}
			
			// Particle life
			if (previousLifetime!=lifetime || 
			    lifetimeValueMethod==VALUEMETHOD.Constant && previousLifetimeValueMethod!=lifetimeValueMethod) {
				SetLifetime(thisInstance, sorting, lifetime);
				initNearestNeighborSeq = 0;
				return false;
			}

			// Lifetime sorting
			if (previousNearestNeighborOrigin!=nearestNeighborOrigin || previousSorting!=sorting) {
				SetLifetime(thisInstance, sorting, lifetime);
				initNearestNeighborSeq = 0;
				return false;
			}

			// Particle total lifetime
			if (lifetimeValueMethod==VALUEMETHOD.RandomBetweenTwoValues) {
				if (previousLifetimeMin!=lifetimeMin || 
				    previousLifetime!=lifetime || 
				    previousLifetimeValueMethod!=lifetimeValueMethod) {
					SetLifetimeSubtraction(thisInstance);
					previousLifetimeMin=lifetimeMin;
				}
			}

			// Particle emission rate
			if (previousEmissionRate!=emissionRate && source!=SOURCEC.Script) {
				SetEmissionRate(thisInstance);
			}
			
			// Particle state change
			if (source==SOURCEC.State && activeState!=previousActiveState) {
				if (states[activeState].positionLength>particleCount)
					SetParticleCount(thisInstance, states[activeState].positionLength);
				previousActiveState = activeState;
			}

			// Refresh delta time
			localDeltaTime = PlaygroundC.globalTime-lastTimeUpdated;

			// Assign all particles into the particle system
			if (!inTransition && particleCache!=null && particleCache.Length>0 && calculate && hasActiveParticles)
				shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
			else if (particleCache.Length==0 && hasActiveParticles) {
				shurikenParticleSystem.Clear();
				hasActiveParticles = false;
				isDoneThread = true;
			}

			// Make sure this particle system is playing
			if (shurikenParticleSystem.isPaused || shurikenParticleSystem.isStopped)
				shurikenParticleSystem.Play();

			// All good!
			return true;
		}
		
		// Initial target position
		public static void SetInitialTargetPosition (PlaygroundParticlesC playgroundParticles, Vector3 position, bool refreshParticleSystem) {
			for (int p = 0; p<playgroundParticles.particleCache.Length; p++) {
				playgroundParticles.playgroundCache.previousTargetPosition[p] = position;
				playgroundParticles.playgroundCache.targetPosition[p] = position;
				playgroundParticles.particleCache[p].size = 0f;
				playgroundParticles.playgroundCache.position[p] = position;
			}
			if (refreshParticleSystem)
				playgroundParticles.particleSystem.SetParticles(playgroundParticles.particleCache, playgroundParticles.particleCache.Length);
		}

		// Set emission of PlaygroundParticlesC object
		public static void Emission (PlaygroundParticlesC playgroundParticles, bool emission, bool callRestEmission) {
			playgroundParticles.previousEmission = emission;
			if (emission) {
				playgroundParticles.hasActiveParticles = true;
				playgroundParticles.threadHadNoActiveParticles = false;
				PlaygroundC.RunAsync (()=>{
					for (int p = 0; p<playgroundParticles.playgroundCache.rebirth.Length; p++) {
						playgroundParticles.playgroundCache.rebirth[p] = true;
						playgroundParticles.playgroundCache.simulate[p] = true;
					}
					if (callRestEmission)
						SetParticleTimeNowWithRestEmission(playgroundParticles);
				});
			}
		}
		
		// Returns the angle between a and b with normal direction
		public static float SignedAngle (Vector3 a, Vector3 b, Vector3 n) {
			return (Vector3.Angle(a, b)*Mathf.Sign(Vector3.Dot(n.normalized, Vector3.Cross(a, b))));
		}
		
		// Returns a random value between negative- and positive Vector3
		public static Vector3 RandomVector3 (System.Random random, Vector3 v1, Vector3 v2) {
			return RandomRange (random, v1, v2);
		}

		/// <summary>
		/// Prepares all values for calculation which is not thread-safe.
		/// </summary>
		/// <returns><c>true</c>, if threaded calculations was prepared, <c>false</c> otherwise.</returns>
		public bool PrepareThreadedCalculations () {

			// Component disabled?
			if (!enabled) return false;
			
			// Has the calculation been paused previously?
			if (cameFromNonCalculatedFrame && hasActiveParticles) {
				StartCoroutine(Boot());
				cameFromNonCalculatedFrame = false;
				return false;
			} 

			// Apply locked position
			if (applyLockPosition) {
				if (lockPositionIsLocal)
					particleSystemTransform.localPosition = lockPosition;
				else
					particleSystemTransform.position = lockPosition;
			}
			
			// Apply locked rotation
			if (applyLockRotation) {
				if (lockRotationIsLocal)
					particleSystemTransform.localRotation = Quaternion.Euler(lockRotation);
				else
					particleSystemTransform.rotation = Quaternion.Euler(lockRotation);
			}
			
			// Apply locked scale
			if (applyLockScale)
				particleSystemTransform.localScale = lockScale;

			// Set delta time
			t = (localDeltaTime*particleTimescale);

			// Update active particle check
			hasActiveParticles = !threadHadNoActiveParticles;
			if (!hasActiveParticles) {
				if (disableOnDone)
					queueEmissionHalt = true;
			}
			
			// Prepare Source positions
			if (PlaygroundC.reference.IsDoneThread() && isDoneThread) {
				stPos = Vector3.zero;
				stRot = Quaternion.identity;
				stSca = new Vector3(1f, 1f, 1f);
				stDir = new Vector3();
			}
			localSpace = (shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local);
			overflow = (overflowOffset!=Vector3.zero);
			skinnedWorldObjectReady = false;
			renderModeStretch = particleSystemRenderer2.renderMode==ParticleSystemRenderMode.Stretch;
			particleSystemInverseRotation = particleSystemTransform.rotation;

			// Prepare turbulence
			if (!onlySourcePositioning && !onlyLifetimePositioning && turbulenceStrength>0 && turbulenceType!=TURBULENCETYPE.None) {
				if (turbulenceType==TURBULENCETYPE.Simplex && turbulenceSimplex==null)
					turbulenceSimplex = new SimplexNoise();
			}
			
			if (emit) {
				switch (source) {
				case SOURCEC.Script:
					
					break;
				case SOURCEC.State:
					if (states.Count>0) {
						if (states[activeState].stateTransform!=null) {
							if (isDoneThread) {
							if (!states[activeState].initialized)
								states[activeState].Initialize();
							
							stPos = states[activeState].stateTransform.position;
							stRot = states[activeState].stateTransform.rotation;
							stSca = states[activeState].stateTransform.localScale;
							}
							if (localSpace && (states[activeState].stateTransform.parent==particleSystemTransform || states[activeState].stateTransform==particleSystemTransform) && isDoneThread) {
								stPos = Vector3.zero;
								stRot = Quaternion.Euler (Vector3.zero);
							}
						}
					} else return false;
					break;
				case SOURCEC.Transform:
					if (isDoneThread) {
						stPos = sourceTransform.position;
						stRot = sourceTransform.rotation;
						stSca = sourceTransform.localScale;
					}
					if (localSpace && sourceTransform==particleSystemTransform && isDoneThread) {
						stPos = Vector3.zero;
						stRot = Quaternion.Euler (Vector3.zero);
					}
					break;
				case SOURCEC.WorldObject:
					
					// Handle vertex data in active World Object
					if (worldObject.gameObject!=null) {
						if (worldObject.gameObject.GetInstanceID()!=worldObject.cachedId || !worldObject.initialized) 
							worldObject = NewWorldObject(worldObject.gameObject.transform);
						if (worldObject.mesh!=null) {
							if (isDoneThread) {
								stPos = worldObject.transform.transform.position;
								stRot = worldObject.transform.transform.rotation;
								stSca = worldObject.transform.transform.localScale;
							}
							if (localSpace && isDoneThread) {
								stPos = Vector3.zero;
								stRot = Quaternion.Euler (Vector3.zero);
							}
							
							worldObject.updateNormals = worldObjectUpdateNormals;
							if (worldObjectUpdateVertices)
								worldObject.Update();
						} else return false;
					} else return false;
					break;
				case SOURCEC.SkinnedWorldObject:
					
					// Handle vertex data in active Skinned World Object
					if (skinnedWorldObject.gameObject!=null) {
						if (skinnedWorldObject.gameObject.GetInstanceID()!=skinnedWorldObject.cachedId)
							skinnedWorldObject = NewSkinnedWorldObject(skinnedWorldObject.gameObject.transform, skinnedWorldObject.downResolution);
					}
					skinnedWorldObjectReady = skinnedWorldObject.gameObject!=null && skinnedWorldObject.mesh!=null;
					if (PlaygroundC.reference.skinnedMeshThreadMethod==ThreadMethodComponent.OneForAll && PlaygroundC.reference.IsFirstUnsafeAutomaticFrames() && !PlaygroundC.reference.IsDoneThreadSkinnedMeshes())
						return false;
					if (skinnedWorldObjectReady) {
						if (isDoneThread) {
							stPos = skinnedWorldObject.transform.position;
							stRot = skinnedWorldObject.transform.rotation;
							stSca = skinnedWorldObject.transform.localScale;
							stDir = skinnedWorldObject.transform.TransformDirection (overflowOffset);
						}
						skinnedWorldObject.updateNormals = worldObjectUpdateNormals;
						
						if (Time.frameCount%PlaygroundC.skinnedUpdateRate==0) {
							if (worldObjectUpdateVertices)
								skinnedWorldObject.MeshUpdate();
							skinnedWorldObject.BoneUpdate();
						}
					} else return false;
					break;
				case SOURCEC.Paint:
					if (paint.initialized) {
						if (isDoneThread) {
							stPos = particleSystemTransform.position;
							stRot = particleSystemTransform.rotation;
							stSca = particleSystemTransform.localScale;
						}
						if (paint.positionLength>0) {
							for (int p = 0; p<particleCache.Length; p++) {
								paint.Update(p);
							}
						} else return false;
					} else {
						paint.Initialize ();
						return false;
					}
					break;
				case SOURCEC.Projection:
					if (projection.projectionTexture!=null && projection.projectionTransform!=null) {
						if (!projection.initialized)
							projection.Initialize();
						
						stPos = projection.projectionTransform.position;
						stRot = projection.projectionTransform.rotation;
						stSca = projection.projectionTransform.localScale;
						
						if (localSpace)
							shurikenParticleSystem.simulationSpace = ParticleSystemSimulationSpace.World;
						
						if (projection.liveUpdate || !projection.hasRefreshed) {
							projection.UpdateSource();
							projection.Update();
							stDir = projection.projectionTransform.TransformDirection (overflowOffset);
							
							projection.hasRefreshed = true;
						}
						
					} else return false;
					break;
				}
			}
			
			// Collision detection (runs on main-thread)
			if (collision)
				Collisions(thisInstance);
			
			// Sync positions to main-thread
			if (syncPositionsOnMainThread) {
				if (particleCache.Length!=playgroundCache.position.Length)
					return false;
				for (int p = 0; p<particleCount; p++) {
					particleCache[p].position = playgroundCache.position[p];
				}
			}
			
			// Prepare Particle colors
			stateReadyForTextureColor = (source==SOURCEC.State && states[activeState].stateTexture!=null);
			
			hasEventManipulatorLocal = false;
			hasEventManipulatorGlobal = false;
			manipulatorEventCount = 0;
			
			// Prepare local manipulators
			for (int m = 0; m<manipulators.Count; m++) {
				manipulators[m].Update();
				manipulators[m].transform.SetLocalPosition(particleSystemTransform);
				manipulators[m].SetLocalTargetsPosition(particleSystemTransform);
				manipulators[m].willAffect = true;
				if (manipulators[m].trackParticles) {
					hasEventManipulatorLocal = true;
					manipulatorEventCount++;
				}
			}
			// Prepare global manipulators from this local space
			for (int m = 0; m<PlaygroundC.reference.manipulators.Count; m++) {
				PlaygroundC.reference.manipulators[m].willAffect = ((PlaygroundC.reference.manipulators[m].affects.value & 1<<particleSystemGameObject.layer)!=0);
				PlaygroundC.reference.manipulators[m].transform.SetLocalPosition(particleSystemTransform);
				if (PlaygroundC.reference.manipulators[m].trackParticles) {
					hasEventManipulatorGlobal = true;
					manipulatorEventCount++;
				}
			}
			
			hasSeveralManipulatorEvents = (manipulatorEventCount>1);
			
			// Prepare events
			hasEvent = events.Count>0;
			hasTimerEvent = false;
			if (hasEvent) {
				for (int i = 0; i<events.Count; i++) {
					events[i].Initialize();
					if (events[i].initializedTarget && events[i].broadcastType!=EVENTBROADCASTC.EventListeners)
						if (!events[i].target.eventControlledBy.Contains (thisInstance))
							events[i].target.eventControlledBy.Add (thisInstance);
					
					if (events[i].eventType==EVENTTYPEC.Time && ((events[i].broadcastType!=EVENTBROADCASTC.EventListeners && events[i].initializedTarget) || events[i].broadcastType==EVENTBROADCASTC.EventListeners && events[i].initializedEvent)) {
						hasTimerEvent = events[i].UpdateTime();
					}
				}
			}

			// All good! Next step is to send all particles to calculation.
			return true;
		}

		public static void NewCalculatedThread (PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.isDoneThread && playgroundParticles.isReadyForThreadedCalculations && !playgroundParticles.IsYieldRefreshing()) {
				playgroundParticles.isDoneThread = false;
				PlaygroundC.RunAsync(()=> {
					if (playgroundParticles.isDoneThread || !playgroundParticles.isReadyForThreadedCalculations) return;
					ThreadedCalculations(playgroundParticles);
				});
			}
		}

		public static void NewCalculatedThread (PlaygroundParticlesC[] playgroundParticles) {
			for (int i = 0; i<playgroundParticles.Length; i++)
				if (playgroundParticles[i].isDoneThread && !playgroundParticles[i].IsYieldRefreshing() && playgroundParticles[i].threadMethod==ThreadMethodLocal.Inherit)
					playgroundParticles[i].isDoneThread = false;
			PlaygroundC.RunAsync(()=> {
				for (int i = 0; i<playgroundParticles.Length; i++) {
					if (!playgroundParticles[i].isDoneThread && PlaygroundC.reference.IsDoneThread() && playgroundParticles[i].isReadyForThreadedCalculations && playgroundParticles[i].threadMethod==ThreadMethodLocal.Inherit) {
						ThreadedCalculations(playgroundParticles[i]);
					}
				}
			});
		}

		/// <summary>
		/// Thread-safe particle calculations.
		/// </summary>
		public bool isDoneThread = true;
		bool threadHadNoActiveParticles = false;
		public static void ThreadedCalculations (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.lastTimeUpdated = PlaygroundC.globalTime;

			if (playgroundParticles.particleCount==0 || 
			    playgroundParticles.playgroundCache.color.Length!=playgroundParticles.particleCount || 
			    playgroundParticles.playgroundCache.targetPosition.Length!=playgroundParticles.particleCount ||
			    playgroundParticles.playgroundCache.targetDirection.Length!=playgroundParticles.particleCount ||
			    playgroundParticles.isYieldRefreshing || !PlaygroundC.IsReady()) {
				playgroundParticles.isDoneThread = true;
				playgroundParticles.threadHadNoActiveParticles = false;
				return;
			}
			if (PlaygroundC.reference.calculate && playgroundParticles.calculate && !playgroundParticles.inTransition && playgroundParticles.hasActiveParticles) {}
			else if (playgroundParticles.source!=SOURCEC.Script) {
				playgroundParticles.isDoneThread = true;
				playgroundParticles.cameFromNonCalculatedFrame = true;
				return;
			} else {
				playgroundParticles.isDoneThread = true;
				return;
			}

			float t = playgroundParticles.t;

			// Prepare variables for particle source positions
			Matrix4x4 stMx = new Matrix4x4();
			Matrix4x4 fMx = new Matrix4x4();
			stMx.SetTRS(playgroundParticles.stPos, playgroundParticles.stRot, playgroundParticles.stSca);
			fMx.SetTRS(Vector3.zero, playgroundParticles.stRot, new Vector3(1f,1f,1f));
			int downResolution = playgroundParticles.skinnedWorldObject.downResolution;
			int downResolutionP;
			bool noActiveParticles = true;

			// Update skinned mesh vertices
			if (playgroundParticles.source==SOURCEC.SkinnedWorldObject && playgroundParticles.skinnedWorldObjectReady && PlaygroundC.reference.skinnedMeshThreadMethod==ThreadMethodComponent.InsideParticleCalculation) {
				playgroundParticles.skinnedWorldObject.Update();
			}

			// Misc
			int pCount = playgroundParticles.particleCache.Length;
			bool applyMask = false;

			// Check that cache is correct
			if (playgroundParticles.playgroundCache.lifetimeSubtraction.Length!=pCount)
				SetLifetimeSubtraction(playgroundParticles);
			if (playgroundParticles.playgroundCache.maskAlpha.Length!=pCount) {
				playgroundParticles.playgroundCache.maskAlpha = new float[pCount];
				playgroundParticles.playgroundCache.isMasked = new bool[pCount];
			}
			if (playgroundParticles.playgroundCache.manipulatorId.Length!=pCount)
				playgroundParticles.playgroundCache.manipulatorId = new int[pCount];
			if (playgroundParticles.playgroundCache.excludeFromManipulatorId.Length!=pCount)
				playgroundParticles.playgroundCache.excludeFromManipulatorId = new int[pCount];
			if (playgroundParticles.playgroundCache.noForce.Length!=pCount)
				playgroundParticles.playgroundCache.noForce = new bool[pCount];
			if (playgroundParticles.playgroundCache.isNonBirthed.Length!=pCount)
				playgroundParticles.playgroundCache.isNonBirthed = new bool[pCount];
			if (playgroundParticles.playgroundCache.isFirstLoop.Length!=pCount)
				playgroundParticles.playgroundCache.isFirstLoop = new bool[pCount];
			if (playgroundParticles.playgroundCache.simulate.Length!=pCount) {
				playgroundParticles.playgroundCache.simulate = new bool[pCount];
				for (int p = 0; p<pCount; p++)
					playgroundParticles.playgroundCache.simulate[p] = true;
			}

			Color lifetimeColor = new Color();
			Vector3 zero = Vector3.zero;
			Vector3 up = Vector3.up;
			
			// Calculation loop
			for (int p = 0; p<playgroundParticles.particleCount; p++) {

				// Check that particle count is correct
				if (pCount != playgroundParticles.particleCache.Length || playgroundParticles.isYieldRefreshing) {
					playgroundParticles.cameFromNonEmissionFrame = false;
					playgroundParticles.isDoneThread = true;
					playgroundParticles.threadHadNoActiveParticles = false;
					return;
				}

				// Check simulation
				if (!playgroundParticles.playgroundCache.simulate[p]) {
					if (playgroundParticles.playgroundCache.rebirth[p] && playgroundParticles.loop) {
						playgroundParticles.InactivateParticle(p);
					}
					continue;
				} else {
					noActiveParticles = false;
				}

				// Prepare variables inside scope
				bool hasLifetimeColors = (playgroundParticles.lifetimeColors.Count>0);
				float manipulatorDistance;
				Vector3 deltaVelocity;
				Vector3 manipulatorPosition;
				float normalizedLife = Mathf.Clamp01 (playgroundParticles.playgroundCache.life[p]/(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p]));
				float evaluatedLife;
				float lifetimePositioningTimeScale = 1f;
				if (playgroundParticles.applyLifetimePositioningTimeScale)
					lifetimePositioningTimeScale = playgroundParticles.lifetimePositioningTimeScale.Evaluate(normalizedLife);

				// Apply particle mask
				if (p<playgroundParticles.particleMask) {
					if (playgroundParticles.playgroundCache.maskAlpha[p]<=0 || playgroundParticles.particleMaskTime<=0) {
						playgroundParticles.playgroundCache.isMasked[p] = true;
						playgroundParticles.playgroundCache.maskAlpha[p] = 0;
						playgroundParticles.particleCache[p].size = 0;
						applyMask = true;
					} else {
						applyMask = false;
						playgroundParticles.playgroundCache.maskAlpha[p] -= (1f/playgroundParticles.particleMaskTime)*playgroundParticles.localDeltaTime;
					}
				} else {
					applyMask = false;
					if (playgroundParticles.playgroundCache.maskAlpha[p]<1f && playgroundParticles.particleMaskTime>0) {
						playgroundParticles.playgroundCache.maskAlpha[p] += (1f/playgroundParticles.particleMaskTime)*playgroundParticles.localDeltaTime;
					} else {
						playgroundParticles.playgroundCache.maskAlpha[p] = 1f;
						playgroundParticles.playgroundCache.isMasked[p] = false;
					}
				}

				// Get this particle's color
				if (!playgroundParticles.playgroundCache.changedByPropertyColor[p] && !playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) {
					switch (playgroundParticles.colorSource) {
					case COLORSOURCEC.Source:
						switch (playgroundParticles.source) {
						case SOURCEC.Script:
							lifetimeColor = playgroundParticles.playgroundCache.scriptedColor[p];
							break;
						case SOURCEC.State:
							if (playgroundParticles.stateReadyForTextureColor)
								lifetimeColor = playgroundParticles.states[playgroundParticles.activeState].GetColor(p);
							else
								lifetimeColor = playgroundParticles.lifetimeColor.Evaluate(normalizedLife);
							break;
						case SOURCEC.Projection:
							lifetimeColor = playgroundParticles.projection.GetColor(p);
							break;
						case SOURCEC.Paint:
							lifetimeColor = playgroundParticles.paint.GetColor(p);
							break;
						default:
							lifetimeColor = playgroundParticles.lifetimeColor.Evaluate(normalizedLife);
							break;
						}
						if (playgroundParticles.sourceUsesLifetimeAlpha)
							lifetimeColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(normalizedLife).a, 0, lifetimeColor.a);
						break;
					case COLORSOURCEC.LifetimeColor:
						lifetimeColor = playgroundParticles.lifetimeColor.Evaluate(normalizedLife);
						break;
					case COLORSOURCEC.LifetimeColors:
						if (hasLifetimeColors)
							lifetimeColor = playgroundParticles.lifetimeColors[playgroundParticles.playgroundCache.lifetimeColorId[p]%playgroundParticles.lifetimeColors.Count].gradient.Evaluate(normalizedLife);
						else
							lifetimeColor = playgroundParticles.lifetimeColor.Evaluate(normalizedLife);
						break;
					}
					
				} else if (playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] || playgroundParticles.playgroundCache.changedByPropertyColorLerp[p]) {
					lifetimeColor = playgroundParticles.particleCache[p].color;
					lifetimeColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(normalizedLife).a, 0, lifetimeColor.a);
				}

				// Assign color to particle
				lifetimeColor*=playgroundParticles.playgroundCache.maskAlpha[p];
				playgroundParticles.particleCache[p].color = lifetimeColor;
				
				// Give Playground Cache its color value
				playgroundParticles.playgroundCache.color[p] = lifetimeColor;

				// Source positions
				if (playgroundParticles.emit) {
					switch (playgroundParticles.source) {
					case SOURCEC.State:
						if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
								playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[p], playgroundParticles.states[playgroundParticles.activeState].GetNormal(p)));
							if (playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale), playgroundParticles.states[playgroundParticles.activeState].GetNormal(p)));
								if (playgroundParticles.applyLifetimePositioningPositionScale)
									playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife);
							}
							if (!playgroundParticles.overflow) {
								playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(playgroundParticles.states[playgroundParticles.activeState].GetPosition(p))+playgroundParticles.playgroundCache.scatterPosition[p];
							} else {
								switch (playgroundParticles.overflowMode) {
								case OVERFLOWMODEC.SourceTransform:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(playgroundParticles.states[playgroundParticles.activeState].GetPosition(p)+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.states[playgroundParticles.activeState].positionLength))+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.World:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(playgroundParticles.states[playgroundParticles.activeState].GetPosition(p))+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.states[playgroundParticles.activeState].positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.SourcePoint:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(playgroundParticles.states[playgroundParticles.activeState].GetPosition(p)+GetOverflow2(playgroundParticles.overflowOffset, playgroundParticles.states[playgroundParticles.activeState].GetNormal(p), p, playgroundParticles.states[playgroundParticles.activeState].positionLength))+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								}
							}
							
							// Local space compensation calculation
							if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
								playgroundParticles.playgroundCache.localSpaceMovementCompensation[p] = playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p];
						}
					break;
					case SOURCEC.Transform:
						if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
								playgroundParticles.playgroundCache.targetDirection[p] = playgroundParticles.stRot*playgroundParticles.playgroundCache.initialLocalVelocity[p];
							if (playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.targetDirection[p] = playgroundParticles.stRot*playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale);
								if (playgroundParticles.applyLifetimePositioningPositionScale)
									playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife*lifetimePositioningTimeScale);
							}
							if (!playgroundParticles.overflow) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.stPos+playgroundParticles.playgroundCache.scatterPosition[p];
							} else {
								switch (playgroundParticles.overflowMode) {
								case OVERFLOWMODEC.SourceTransform:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(GetOverflow2(playgroundParticles.overflowOffset, p, 1))+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.World:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.stPos+GetOverflow2(playgroundParticles.overflowOffset, p, 1)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.SourcePoint:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(GetOverflow2(playgroundParticles.overflowOffset, p, 1))+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								}
							}
							
							// Local space compensation calculation
							if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
								playgroundParticles.playgroundCache.localSpaceMovementCompensation[p] = playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p];
						}
					break;
					case SOURCEC.WorldObject:
						if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
								playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[p], playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length]));
							if (playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale), playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length]));
								if (playgroundParticles.applyLifetimePositioningPositionScale)
									playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife);
							}
							if (!playgroundParticles.overflow) {
								playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(
									playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]
									)+playgroundParticles.playgroundCache.scatterPosition[p];
							} else {
								switch (playgroundParticles.overflowMode) {
								case OVERFLOWMODEC.SourceTransform:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(
										playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.worldObject.vertexPositions.Length)
										)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.World:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(
										playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]
										)+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.worldObject.vertexPositions.Length)+
										playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.SourcePoint:
									playgroundParticles.playgroundCache.targetPosition[p] = stMx.MultiplyPoint3x4(
										playgroundParticles.worldObject.vertexPositions[p%playgroundParticles.worldObject.vertexPositions.Length]+GetOverflow2(playgroundParticles.overflowOffset, playgroundParticles.worldObject.normals[p%playgroundParticles.worldObject.normals.Length], p, playgroundParticles.worldObject.vertexPositions.Length)
										)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								}
							}
							
							// Local space compensation calculation
							if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
								playgroundParticles.playgroundCache.localSpaceMovementCompensation[p] = playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p];
						}
					break;
					case SOURCEC.SkinnedWorldObject:
						if (playgroundParticles.skinnedWorldObjectReady) {
							if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
								if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
									playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[p], playgroundParticles.skinnedWorldObject.normals[p%playgroundParticles.skinnedWorldObject.normals.Length]));
								if (playgroundParticles.onlyLifetimePositioning) {
									playgroundParticles.playgroundCache.targetDirection[p] = fMx.MultiplyPoint3x4(Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale), playgroundParticles.skinnedWorldObject.normals[p%playgroundParticles.skinnedWorldObject.normals.Length]));
									if (playgroundParticles.applyLifetimePositioningPositionScale)
										playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife);
								}
								downResolutionP = Mathf.RoundToInt(p*downResolution)%playgroundParticles.skinnedWorldObject.vertexPositions.Length;
								if (!playgroundParticles.overflow) {
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[downResolutionP]+playgroundParticles.playgroundCache.scatterPosition[p];
								} else {
									switch (playgroundParticles.overflowMode) {
									case OVERFLOWMODEC.SourceTransform:
										playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[downResolutionP]+
											GetOverflow2(
												playgroundParticles.stDir,
												p, 
												playgroundParticles.skinnedWorldObject.vertexPositions.Length/downResolution
												)+playgroundParticles.playgroundCache.scatterPosition[p];
										break;
									case OVERFLOWMODEC.World:
										playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[downResolutionP]+
											GetOverflow2(
												playgroundParticles.overflowOffset,
												p, 
												playgroundParticles.skinnedWorldObject.vertexPositions.Length/downResolution
												)+playgroundParticles.playgroundCache.scatterPosition[p];
										break;
									case OVERFLOWMODEC.SourcePoint:
										playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.skinnedWorldObject.vertexPositions[downResolutionP]+
											GetOverflow2(
												playgroundParticles.overflowOffset,
												playgroundParticles.skinnedWorldObject.normals[downResolutionP],
												p, 
												playgroundParticles.skinnedWorldObject.vertexPositions.Length/downResolution
												)+playgroundParticles.playgroundCache.scatterPosition[p];
										break;
									}
								}
								
								// Local space compensation calculation
								if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation)
									playgroundParticles.playgroundCache.localSpaceMovementCompensation[p] = playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p];
							}
						}
					
					break;
					case SOURCEC.Projection:
						if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];

							if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
								playgroundParticles.playgroundCache.targetDirection[p] = Vector3Scale(playgroundParticles.projection.GetNormal(p), playgroundParticles.playgroundCache.initialLocalVelocity[p]);
							if (playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.targetDirection[p] = Vector3Scale(playgroundParticles.projection.GetNormal(p), playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale));
								if (playgroundParticles.applyLifetimePositioningPositionScale)
									playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife);
							}
							if (!playgroundParticles.overflow) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+playgroundParticles.playgroundCache.scatterPosition[p];
							} else {
								switch (playgroundParticles.overflowMode) {
								case OVERFLOWMODEC.SourceTransform:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+GetOverflow2(playgroundParticles.stDir, p, playgroundParticles.projection.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.World:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.projection.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.SourcePoint:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.projection.GetPosition(p)+GetOverflow2(Vector3Scale(playgroundParticles.stDir, playgroundParticles.projection.GetNormal(p)), p, playgroundParticles.projection.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								}
							}
						}

					break;
					case SOURCEC.Paint:
						if (playgroundParticles.playgroundCache.rebirth[p] || playgroundParticles.onlySourcePositioning || playgroundParticles.onlyLifetimePositioning) {
							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
							if (playgroundParticles.applyInitialLocalVelocity && !playgroundParticles.onlyLifetimePositioning)
								playgroundParticles.playgroundCache.targetDirection[p] = playgroundParticles.paint.GetRotation(p)*Vector3Scale(playgroundParticles.playgroundCache.initialLocalVelocity[p], playgroundParticles.paint.GetNormal(p));
							if (playgroundParticles.onlyLifetimePositioning) {
								playgroundParticles.playgroundCache.targetDirection[p] = playgroundParticles.paint.GetRotation(p)*Vector3Scale(playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale), playgroundParticles.paint.GetNormal(p));
								if (playgroundParticles.applyLifetimePositioningPositionScale)
									playgroundParticles.playgroundCache.targetDirection[p] *= playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife);
							}
							if (!playgroundParticles.overflow) {
								playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+playgroundParticles.playgroundCache.scatterPosition[p];
							} else {
								switch (playgroundParticles.overflowMode) {
								case OVERFLOWMODEC.SourceTransform:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+GetOverflow2(playgroundParticles.paint.GetRotation(p)*playgroundParticles.overflowOffset, p, playgroundParticles.paint.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.World:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+GetOverflow2(playgroundParticles.overflowOffset, p, playgroundParticles.paint.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								case OVERFLOWMODEC.SourcePoint:
									playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.paint.GetPosition(p)+playgroundParticles.paint.GetRotation(p)*GetOverflow2(playgroundParticles.overflowOffset, playgroundParticles.paint.GetNormal(p), p, playgroundParticles.paint.positionLength)+playgroundParticles.playgroundCache.scatterPosition[p];
									break;
								}
							}
							
							// Local space compensation calculation
							if (playgroundParticles.localSpace && playgroundParticles.applyLocalSpaceMovementCompensation) 
								playgroundParticles.playgroundCache.localSpaceMovementCompensation[p] = playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p];
						}
					break;
					}
				}

				// Set initial particle values if life is 0
				if (playgroundParticles.playgroundCache.life[p]==0) {
					if (!playgroundParticles.onlyLifetimePositioning)
						playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.targetPosition[p];
					playgroundParticles.playgroundCache.initialColor[p] = lifetimeColor;
				}

				// Particle lifetime, velocity and manipulators
				if (playgroundParticles.playgroundCache.rebirth[p]) {
					
					// Particle is alive
					if (playgroundParticles.playgroundCache.birth[p]+playgroundParticles.playgroundCache.life[p]<=PlaygroundC.globalTime+(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p]) && (!playgroundParticles.playgroundCache.isNonBirthed[p] || playgroundParticles.onlyLifetimePositioning || playgroundParticles.onlySourcePositioning)) {

						// Lifetime size
						if (!playgroundParticles.playgroundCache.changedByPropertySize[p]) {
							if (playgroundParticles.applyLifetimeSize)
								playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p]*playgroundParticles.lifetimeSize.Evaluate(normalizedLife)*playgroundParticles.scale;
							else
								playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p]*playgroundParticles.scale;
						}

						// Local Manipulators
						for (int m = 0; m<playgroundParticles.manipulators.Count; m++) {
							if (playgroundParticles.manipulators[m].transform!=null) {
								manipulatorPosition = playgroundParticles.localSpace?playgroundParticles.manipulators[m].transform.localPosition:playgroundParticles.manipulators[m].transform.position;
								manipulatorDistance = playgroundParticles.manipulators[m].strengthDistanceEffect>0?Vector3.Distance (manipulatorPosition, playgroundParticles.playgroundCache.position[p])/playgroundParticles.manipulators[m].strengthDistanceEffect:10f;
								CalculateManipulator(playgroundParticles, playgroundParticles.manipulators[m], p, t, playgroundParticles.playgroundCache.life[p], playgroundParticles.playgroundCache.position[p], manipulatorPosition, manipulatorDistance, playgroundParticles.localSpace);
							}
						}

						// Set particle size
						playgroundParticles.particleCache[p].size = !applyMask?playgroundParticles.playgroundCache.size[p]:0;

						if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning) {
							
							if (!playgroundParticles.playgroundCache.noForce[p] && playgroundParticles.playgroundCache.life[p]!=0) {

								// Velocity bending
								if (playgroundParticles.applyVelocityBending) {
									if (playgroundParticles.velocityBendingType==VELOCITYBENDINGTYPEC.SourcePosition) {
										playgroundParticles.playgroundCache.velocity[p] += Vector3.Reflect(
											new Vector3(
											playgroundParticles.playgroundCache.velocity[p].x*playgroundParticles.velocityBending.x,
											playgroundParticles.playgroundCache.velocity[p].y*playgroundParticles.velocityBending.y, 
											playgroundParticles.playgroundCache.velocity[p].z*playgroundParticles.velocityBending.z
											),
											(playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.position[p]).normalized
											)*t;
									} else {
										playgroundParticles.playgroundCache.velocity[p] += Vector3.Reflect(
											new Vector3(
											playgroundParticles.playgroundCache.velocity[p].x*playgroundParticles.velocityBending.x,
											playgroundParticles.playgroundCache.velocity[p].y*playgroundParticles.velocityBending.y, 
											playgroundParticles.playgroundCache.velocity[p].z*playgroundParticles.velocityBending.z
											),
											(playgroundParticles.playgroundCache.previousParticlePosition[p]-playgroundParticles.playgroundCache.position[p]).normalized
											)*t;
									}
								}
								
								// Delta velocity
								if (!playgroundParticles.cameFromNonEmissionFrame && playgroundParticles.calculateDeltaMovement && playgroundParticles.playgroundCache.life[p]==0 && playgroundParticles.source!=SOURCEC.Script) {
									deltaVelocity = playgroundParticles.playgroundCache.targetPosition[p]-(playgroundParticles.playgroundCache.previousTargetPosition[p]);
									playgroundParticles.playgroundCache.velocity[p] += deltaVelocity*playgroundParticles.deltaMovementStrength;
								}

								// Set previous target position (used by delta velocity & local space movement compensation)
								playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
								
								// Gravity
								playgroundParticles.playgroundCache.velocity[p] -= playgroundParticles.gravity*t;
								
								// Lifetime additive velocity
								if (playgroundParticles.applyLifetimeVelocity) 
									playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.lifetimeVelocity.Evaluate(normalizedLife, playgroundParticles.lifetimeVelocityScale)*t;

								// Turbulence inside the calculation loop
								if (playgroundParticles.HasTurbulence() && PlaygroundC.reference.turbulenceThreadMethod==ThreadMethodComponent.InsideParticleCalculation) {
								if (!playgroundParticles.playgroundCache.noForce[p])
									Turbulence (playgroundParticles, playgroundParticles.turbulenceSimplex, p, playgroundParticles.t, playgroundParticles.turbulenceType, playgroundParticles.turbulenceTimeScale, playgroundParticles.turbulenceScale/playgroundParticles.velocityScale, playgroundParticles.turbulenceStrength, playgroundParticles.turbulenceApplyLifetimeStrength, playgroundParticles.turbulenceLifetimeStrength);
								}

								// Damping, max velocity, constraints and final positioning
								if (playgroundParticles.playgroundCache.velocity[p]!=zero) {

									// Max velocity
									if (playgroundParticles.playgroundCache.velocity[p].sqrMagnitude>playgroundParticles.maxVelocity)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.ClampMagnitude(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.maxVelocity);

									// Damping
									if (playgroundParticles.damping>0)
										playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], zero, playgroundParticles.damping*t);

									// Axis constraints
									if (playgroundParticles.axisConstraints.x)
										playgroundParticles.playgroundCache.velocity[p].x = 0;
									if (playgroundParticles.axisConstraints.y)
										playgroundParticles.playgroundCache.velocity[p].y = 0;
									if (playgroundParticles.axisConstraints.z)
										playgroundParticles.playgroundCache.velocity[p].z = 0;
									
									
									// Set calculated collision position 
									playgroundParticles.playgroundCache.collisionParticlePosition[p] = playgroundParticles.playgroundCache.previousParticlePosition[p];

									
									// Relocate
									playgroundParticles.playgroundCache.position[p] += (playgroundParticles.playgroundCache.velocity[p]*playgroundParticles.velocityScale)*t;
									if (playgroundParticles.applyLocalSpaceMovementCompensation) {
										if (!playgroundParticles.applyMovementCompensationLifetimeStrength)
											playgroundParticles.playgroundCache.position[p] += playgroundParticles.playgroundCache.localSpaceMovementCompensation[p];
										else
											playgroundParticles.playgroundCache.position[p] += playgroundParticles.playgroundCache.localSpaceMovementCompensation[p]*playgroundParticles.movementCompensationLifetimeStrength.Evaluate(normalizedLife);
									}

										// Set particle velocity to be able to stretch towards movement
										if (playgroundParticles.renderModeStretch) {
											if (playgroundParticles.applyLifetimeStretching)
												playgroundParticles.particleCache[p].velocity = Vector3.Slerp (playgroundParticles.particleCache[p].velocity, playgroundParticles.playgroundCache.position[p]-playgroundParticles.playgroundCache.previousParticlePosition[p], t*playgroundParticles.stretchSpeed)*playgroundParticles.stretchLifetime.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime);
											else
												playgroundParticles.particleCache[p].velocity = Vector3.Slerp (playgroundParticles.particleCache[p].velocity, playgroundParticles.playgroundCache.position[p]-playgroundParticles.playgroundCache.previousParticlePosition[p], t*playgroundParticles.stretchSpeed);
										}

								}
							} else playgroundParticles.particleCache[p].velocity = Vector3.zero;
						} else {
							
							// Only Source Positioning / Lifetime Positioning
							// Set particle velocity to be able to stretch towards movement
							if (playgroundParticles.renderModeStretch) {
								if (playgroundParticles.applyLifetimeStretching)
									playgroundParticles.particleCache[p].velocity = Vector3.Slerp (playgroundParticles.particleCache[p].velocity, playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p], t*playgroundParticles.stretchSpeed)*playgroundParticles.stretchLifetime.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime);
								else
									playgroundParticles.particleCache[p].velocity = Vector3.Slerp (playgroundParticles.particleCache[p].velocity, playgroundParticles.playgroundCache.targetPosition[p]-playgroundParticles.playgroundCache.previousTargetPosition[p], t*playgroundParticles.stretchSpeed);
							}

							if (playgroundParticles.onlyLifetimePositioning && !playgroundParticles.onlySourcePositioning) {
								if (!playgroundParticles.playgroundCache.changedByPropertyTarget[p]) {
									if (playgroundParticles.lifetimePositioningUsesSourceDirection && playgroundParticles.source!=SOURCEC.Script) {
										playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.targetPosition[p]+(playgroundParticles.playgroundCache.targetDirection[p]);
									} else {
										if (!playgroundParticles.applyLifetimePositioningPositionScale) {
											playgroundParticles.playgroundCache.position[p] = 
											playgroundParticles.playgroundCache.targetPosition[p]+
												playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale);
										} else {
											playgroundParticles.playgroundCache.position[p] = 
											playgroundParticles.playgroundCache.targetPosition[p]+
											playgroundParticles.lifetimePositioning.Evaluate(normalizedLife*lifetimePositioningTimeScale, playgroundParticles.lifetimePositioningScale)*
											playgroundParticles.lifetimePositioningPositionScale.Evaluate(normalizedLife*lifetimePositioningTimeScale);
										}
									}
								}
							} else if (playgroundParticles.source!=SOURCEC.Script) {
								playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.targetPosition[p];
							}

							playgroundParticles.playgroundCache.previousTargetPosition[p] = playgroundParticles.playgroundCache.targetPosition[p];

						}

						// Rotation
						if (!playgroundParticles.rotateTowardsDirection)
							playgroundParticles.playgroundCache.rotation[p] += playgroundParticles.playgroundCache.rotationSpeed[p]*t;
						else if (playgroundParticles.playgroundCache.life[p]!=0) {
							playgroundParticles.playgroundCache.rotation[p] = playgroundParticles.playgroundCache.initialRotation[p]+SignedAngle(
								up, 
								playgroundParticles.playgroundCache.position[p]-playgroundParticles.playgroundCache.previousParticlePosition[p],
								playgroundParticles.rotationNormal
							);
						}
						
						if (playgroundParticles.playgroundCache.life[p]>0)
							playgroundParticles.particleCache[p].rotation = playgroundParticles.playgroundCache.rotation[p];
						
						// Set previous particle position
						playgroundParticles.playgroundCache.previousParticlePosition[p] = playgroundParticles.playgroundCache.position[p];


						// Send timed event
						if (playgroundParticles.hasTimerEvent)
							playgroundParticles.SendEvent(EVENTTYPEC.Time, p);
					} else {
						playgroundParticles.playgroundCache.position[p] = PlaygroundC.initialTargetPosition;
						playgroundParticles.particleCache[p].size = 0;
					}


					// Calculate lifetime
					evaluatedLife = (PlaygroundC.globalTime-playgroundParticles.playgroundCache.birth[p])/(playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p]);
					
					// Lifetime
					if (playgroundParticles.playgroundCache.life[p]<playgroundParticles.lifetime) {
						playgroundParticles.playgroundCache.life[p] = (playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p])*evaluatedLife;
						playgroundParticles.particleCache[p].lifetime = Mathf.Clamp(playgroundParticles.lifetime - playgroundParticles.playgroundCache.life[p], .1f, playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p]);

						if (playgroundParticles.lifetimeValueMethod==VALUEMETHOD.RandomBetweenTwoValues && playgroundParticles.playgroundCache.life[p]>playgroundParticles.lifetime-playgroundParticles.playgroundCache.lifetimeSubtraction[p]) {
					
							// Send death event for particles with lifetime subtraction
							if (playgroundParticles.hasEvent && !playgroundParticles.playgroundCache.isNonBirthed[p])
								SendDeathEvents(playgroundParticles, p);

							playgroundParticles.playgroundCache.position[p] = PlaygroundC.initialTargetPosition;
							playgroundParticles.particleCache[p].size = 0;
						}
					} else {
						
						// Loop exceeded
						if (!playgroundParticles.loop && PlaygroundC.globalTime>playgroundParticles.simulationStarted+playgroundParticles.lifetime-.01f) {
							playgroundParticles.loopExceeded = true;

							if (playgroundParticles.loopExceededOnParticle==p && evaluatedLife>2f) {
								if (playgroundParticles.disableOnDone)
									playgroundParticles.queueEmissionHalt = true;
								playgroundParticles.threadHadNoActiveParticles = true;
								playgroundParticles.hasActiveParticles = false;
							}
							if (playgroundParticles.loopExceededOnParticle==-1)
								playgroundParticles.loopExceededOnParticle = p;
						}
						
						// Send death event for particles with full lifetime length
						if (playgroundParticles.hasEvent && !playgroundParticles.playgroundCache.isNonBirthed[p])
							SendDeathEvents(playgroundParticles, p);

						// New cycle begins
						if (PlaygroundC.globalTime>=playgroundParticles.playgroundCache.birth[p]+playgroundParticles.playgroundCache.birthDelay[p] && !playgroundParticles.loopExceeded && playgroundParticles.source!=SOURCEC.Script && playgroundParticles.emit) {
							if (!playgroundParticles.playgroundCache.changedByPropertyDeath[p] || playgroundParticles.playgroundCache.changedByPropertyDeath[p] && PlaygroundC.globalTime>playgroundParticles.playgroundCache.death[p]) {
								Rebirth(playgroundParticles, p, playgroundParticles.internalRandom01);
							} else {
								playgroundParticles.playgroundCache.position[p] = PlaygroundC.initialTargetPosition;
								playgroundParticles.particleCache[p].size = 0;
							}
						} else {
							playgroundParticles.InactivateParticle(p);
						}

					}

				} else {
					
					// Particle is set to not rebirth
					playgroundParticles.InactivateParticle(p);
				}

				// Set particle position if no sync
				if (!playgroundParticles.syncPositionsOnMainThread) {
					playgroundParticles.particleCache[p].position = playgroundParticles.playgroundCache.position[p];
				}

			}// <- Particle loop ends here

			// Reset for next frame
			playgroundParticles.threadHadNoActiveParticles = noActiveParticles;
			playgroundParticles.cameFromNonEmissionFrame = false;
			playgroundParticles.isDoneThread = true;

			// Turbulence calculated here when using PlaygroundC.turbulenceThreadMethod of ThreadMethodComponent.OnePerSystem
			if (playgroundParticles.HasTurbulence() && PlaygroundC.reference.turbulenceThreadMethod==ThreadMethodComponent.OnePerSystem) {
				PlaygroundC.RunAsync(()=>{
					for (int p = 0; p<playgroundParticles.particleCount; p++) {
						if (!playgroundParticles.playgroundCache.noForce[p])
							Turbulence (playgroundParticles, playgroundParticles.turbulenceSimplex, p, playgroundParticles.t, playgroundParticles.turbulenceType, playgroundParticles.turbulenceTimeScale, playgroundParticles.turbulenceScale/playgroundParticles.velocityScale, playgroundParticles.turbulenceStrength, playgroundParticles.turbulenceApplyLifetimeStrength, playgroundParticles.turbulenceLifetimeStrength);
					}
				});
			}
		}

		/// <summary>
		/// Sends the death events.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="p">Particle index.</param>
		public static void SendDeathEvents (PlaygroundParticlesC playgroundParticles, int p) {
			if (playgroundParticles.playgroundCache.life[p]>0 && !playgroundParticles.playgroundCache.isNonBirthed[p]) {
				if (playgroundParticles.loop || (!playgroundParticles.loop && playgroundParticles.playgroundCache.isFirstLoop[p])) {
					playgroundParticles.SendEvent(EVENTTYPEC.Death, p);
				}
			}
			if (playgroundParticles.hasEventManipulatorLocal) {
				for (int i = 0; i<playgroundParticles.manipulators.Count; i++) {
					if (playgroundParticles.manipulators[i].trackParticles &&
					    playgroundParticles.manipulators[i].RemoveParticle (playgroundParticles.particleSystemId, p)) {
						
						if (playgroundParticles.manipulators[i].sendEventDeath) {
							playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[i].manipulatorEventParticle, p);
							playgroundParticles.manipulators[i].SendParticleEventDeath();
						}
					}
				}
			}
			if (playgroundParticles.hasEventManipulatorGlobal) {
				for (int i = 0; i<PlaygroundC.reference.manipulators.Count; i++) {
					if (PlaygroundC.reference.manipulators[i].trackParticles &&
					    PlaygroundC.reference.manipulators[i].RemoveParticle (playgroundParticles.particleSystemId, p)) {
						
						if (PlaygroundC.reference.manipulators[i].sendEventDeath) {
							playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[i].manipulatorEventParticle, p);
							PlaygroundC.reference.manipulators[i].SendParticleEventDeath();
						}
					}
				}
			}
			playgroundParticles.playgroundCache.isNonBirthed[p] = true;
		}
		
		
		public static void Turbulence (PlaygroundParticlesC playgroundParticles, SimplexNoise turbulenceSimplex, int p, float t, TURBULENCETYPE turbulenceType, float turbulenceTimeScale, float turbulenceScale, float turbulenceStrength, bool turbulenceApplyLifetimeStrength, AnimationCurve turbulenceLifetimeStrength) {
			float turbulenceStrengthMultiplier = 1f;
			float zeroFixX = 1f;
			float zeroFixY = 2f; 
			float zeroFixZ = 3f;

			if (playgroundParticles.playgroundCache.simulate[p]) {

				if (turbulenceType==TURBULENCETYPE.Simplex) {

					// Simplex Noise
					if (turbulenceTimeScale>0) {
						if (playgroundParticles.turbulenceApplyLifetimeStrength)
							turbulenceStrengthMultiplier = playgroundParticles.turbulenceLifetimeStrength.Evaluate (Mathf.Clamp01(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime));
						if (turbulenceStrengthMultiplier>0) {
							if (!playgroundParticles.axisConstraints.x)
								playgroundParticles.playgroundCache.velocity[p].x += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale,
									PlaygroundC.globalTime*turbulenceTimeScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.y)
								playgroundParticles.playgroundCache.velocity[p].y += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale,
									PlaygroundC.globalTime*turbulenceTimeScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.z)
								playgroundParticles.playgroundCache.velocity[p].z += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale,
									PlaygroundC.globalTime*turbulenceTimeScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
						}
					} else {
						if (turbulenceStrengthMultiplier>0) {
							if (!playgroundParticles.axisConstraints.x)
								playgroundParticles.playgroundCache.velocity[p].x += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.y)
								playgroundParticles.playgroundCache.velocity[p].y += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.z)
								playgroundParticles.playgroundCache.velocity[p].z += (float)turbulenceSimplex.noise(
									(playgroundParticles.playgroundCache.position[p].z+zeroFixZ)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].x+zeroFixX)*turbulenceScale,
									(playgroundParticles.playgroundCache.position[p].y+zeroFixY)*turbulenceScale
									)*turbulenceStrength*t*turbulenceStrengthMultiplier;
						}
					}
				} else {

					// Perlin Noise
					if (playgroundParticles.turbulenceStrength>0) {
						if (playgroundParticles.turbulenceApplyLifetimeStrength)
							turbulenceStrengthMultiplier = playgroundParticles.turbulenceLifetimeStrength.Evaluate (Mathf.Clamp01(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime));
						if (turbulenceStrengthMultiplier>0) {
							if (!playgroundParticles.axisConstraints.x)
								playgroundParticles.playgroundCache.velocity[p].x += (Mathf.PerlinNoise (
									PlaygroundC.globalTime*turbulenceTimeScale,
									playgroundParticles.playgroundCache.position[p].z*turbulenceScale
									)-.5f)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.y)
								playgroundParticles.playgroundCache.velocity[p].y += (Mathf.PerlinNoise (
									PlaygroundC.globalTime*turbulenceTimeScale,
									playgroundParticles.playgroundCache.position[p].x*turbulenceScale
									
									)-.5f)*turbulenceStrength*t*turbulenceStrengthMultiplier;
							if (!playgroundParticles.axisConstraints.z)
								playgroundParticles.playgroundCache.velocity[p].z += (Mathf.PerlinNoise (
									PlaygroundC.globalTime*turbulenceTimeScale,
									playgroundParticles.playgroundCache.position[p].y*turbulenceScale
									
									)-.5f)*turbulenceStrength*t*turbulenceStrengthMultiplier;
						}
					}
				}
			}
		}

		public static void Collisions (PlaygroundParticlesC playgroundParticles) {

			// Particle collisions (must currently run on main-thread due to the Physics.Raycast dependency)
			if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning && playgroundParticles.collisionRadius>0) {
				Ray ray = new Ray();
				float distance;
				bool is3d = playgroundParticles.collisionType==COLLISIONTYPEC.Physics3D;
				RaycastHit hitInfo;
				RaycastHit2D hitInfo2D;
				bool hasEvents = playgroundParticles.events.Count>0;
				Vector3 preCollisionVelocity;

				// Prepare the infinite collision planes
				if (playgroundParticles.collision && playgroundParticles.collisionRadius>0 && playgroundParticles.colliders.Count>0) {
					for (int c = 0; c<playgroundParticles.colliders.Count; c++) {
						playgroundParticles.colliders[c].UpdatePlane();
					}
				}

				// Check cache length
				if (playgroundParticles.playgroundCache.noForce.Length!=playgroundParticles.particleCount)
					playgroundParticles.playgroundCache.noForce = new bool[playgroundParticles.particleCount];

				for (int p = 0; p<playgroundParticles.particleCount; p++) {

					if (playgroundParticles.playgroundCache.life[p]==0 || playgroundParticles.playgroundCache.life[p]>=playgroundParticles.lifetime || playgroundParticles.playgroundCache.noForce[p]) continue;

					// Playground Plane colliders (never exceed these)
					for (int c = 0; c<playgroundParticles.colliders.Count; c++) {
						if (playgroundParticles.colliders[c].enabled && playgroundParticles.colliders[c].transform && !playgroundParticles.colliders[c].plane.GetSide(playgroundParticles.playgroundCache.position[p])) {
							
							// Set particle to location
							ray.origin = playgroundParticles.playgroundCache.position[p];
							ray.direction = playgroundParticles.colliders[c].plane.normal;
							if (playgroundParticles.colliders[c].plane.Raycast(ray, out distance))
								playgroundParticles.playgroundCache.position[p] = ray.GetPoint(distance);

							// Store velocity before collision
							preCollisionVelocity = playgroundParticles.playgroundCache.velocity[p];

							// Reflect particle
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.colliders[c].plane.normal+RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
							
							// Apply lifetime loss
							if (playgroundParticles.lifetimeLoss>0) {
								playgroundParticles.playgroundCache.birth[p] -= playgroundParticles.playgroundCache.life[p]/(1f-playgroundParticles.lifetimeLoss);
								playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
							}

							// Send event
							if (hasEvents)
								playgroundParticles.SendEvent(EVENTTYPEC.Collision, p, preCollisionVelocity, playgroundParticles.colliders[c].transform);
						}
					}
					
					// Colliders in scene
					if (playgroundParticles.playgroundCache.velocity[p].magnitude>PlaygroundC.collisionSleepVelocity) {

						// Collide by checking for potential passed collider in the direction of this particle's velocity from the previous frame
						if (is3d) {
							if (Physics.Raycast(
								playgroundParticles.playgroundCache.collisionParticlePosition[p], 
								(playgroundParticles.playgroundCache.position[p]-playgroundParticles.playgroundCache.collisionParticlePosition[p]).normalized, 
								out hitInfo, 
								Vector3.Distance(playgroundParticles.playgroundCache.collisionParticlePosition[p], playgroundParticles.playgroundCache.position[p])+playgroundParticles.collisionRadius, 
								playgroundParticles.collisionMask)) 
							{
								
								// Set particle to location
								playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.collisionParticlePosition[p];

								// Store velocity before collision
								preCollisionVelocity = playgroundParticles.playgroundCache.velocity[p];

								// Reflect particle
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], hitInfo.normal+RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
								
								// Apply lifetime loss
								if (playgroundParticles.lifetimeLoss>0) {
									playgroundParticles.playgroundCache.birth[p] -= playgroundParticles.playgroundCache.life[p]/(1f-playgroundParticles.lifetimeLoss);
									playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
								}
								
								// Add force to rigidbody
								if (playgroundParticles.affectRigidbodies && hitInfo.rigidbody)
									hitInfo.rigidbody.AddForceAtPosition(playgroundParticles.playgroundCache.velocity[p]*playgroundParticles.mass, playgroundParticles.playgroundCache.position[p]);

								// Send event
								if (hasEvents)
									playgroundParticles.SendEvent(EVENTTYPEC.Collision, p, preCollisionVelocity, hitInfo.transform, hitInfo.collider);
								if (playgroundParticles.hasEventManipulatorLocal) {
									for (int i = 0; i<playgroundParticles.manipulators.Count; i++) {
										if (playgroundParticles.manipulators[i].trackParticles &&
										    playgroundParticles.manipulators[i].sendEventCollision &&
										    playgroundParticles.manipulators[i].Contains (playgroundParticles.playgroundCache.position[p], playgroundParticles.manipulators[i].transform.position)) {

											playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[i].manipulatorEventParticle, p);
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionCollider = hitInfo.collider;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionParticlePosition = hitInfo.point;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionTransform = hitInfo.transform;
											playgroundParticles.manipulators[i].SendParticleEventCollision();
										}
									}
								}
								if (playgroundParticles.hasEventManipulatorGlobal) {
									for (int i = 0; i<PlaygroundC.reference.manipulators.Count; i++) {
										if (PlaygroundC.reference.manipulators[i].trackParticles &&
										    PlaygroundC.reference.manipulators[i].sendEventCollision &&
										    PlaygroundC.reference.manipulators[i].Contains (playgroundParticles.playgroundCache.position[p], PlaygroundC.reference.manipulators[i].transform.position)) {

											playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[i].manipulatorEventParticle, p);
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionCollider = hitInfo.collider;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionParticlePosition = hitInfo.point;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionTransform = hitInfo.transform;
											PlaygroundC.reference.manipulators[i].SendParticleEventCollision();
										}
									}
								}
							}
						} else {
							hitInfo2D = Physics2D.Raycast(
								playgroundParticles.playgroundCache.collisionParticlePosition[p], 
								(playgroundParticles.playgroundCache.position[p]-playgroundParticles.playgroundCache.collisionParticlePosition[p]).normalized, 
								Vector3.Distance(playgroundParticles.playgroundCache.collisionParticlePosition[p], playgroundParticles.playgroundCache.position[p])+playgroundParticles.collisionRadius, 
								playgroundParticles.collisionMask,
								playgroundParticles.minCollisionDepth,
								playgroundParticles.maxCollisionDepth
							);
							if (hitInfo2D.collider!=null) {
								
								// Set particle to location
								playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.collisionParticlePosition[p];

								// Store velocity before collision
								preCollisionVelocity = playgroundParticles.playgroundCache.velocity[p];

								// Reflect particle
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Reflect(playgroundParticles.playgroundCache.velocity[p], (Vector3)hitInfo2D.normal+RandomVector3(playgroundParticles.internalRandom01, playgroundParticles.bounceRandomMin, playgroundParticles.bounceRandomMax))*playgroundParticles.bounciness;
								
								// Apply lifetime loss
								if (playgroundParticles.lifetimeLoss>0) {
									playgroundParticles.playgroundCache.birth[p] -= playgroundParticles.playgroundCache.life[p]/(1f-playgroundParticles.lifetimeLoss);
									playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
								}
								
								// Add force to rigidbody
								if (playgroundParticles.affectRigidbodies && hitInfo2D.rigidbody)
									hitInfo2D.rigidbody.AddForceAtPosition(playgroundParticles.playgroundCache.velocity[p]*playgroundParticles.mass, playgroundParticles.playgroundCache.position[p]);

								// Send event
								if (hasEvents)
									playgroundParticles.SendEvent(EVENTTYPEC.Collision, p, preCollisionVelocity, hitInfo2D.transform, hitInfo2D.collider);
								if (playgroundParticles.hasEventManipulatorLocal) {
									for (int i = 0; i<playgroundParticles.manipulators.Count; i++) {
										if (playgroundParticles.manipulators[i].trackParticles &&
										    playgroundParticles.manipulators[i].sendEventCollision &&
										    playgroundParticles.manipulators[i].Contains (playgroundParticles.playgroundCache.position[p], playgroundParticles.manipulators[i].transform.position)) {

											playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[i].manipulatorEventParticle, p);
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionCollider2D = hitInfo2D.collider;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionParticlePosition = hitInfo2D.point;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionTransform = hitInfo2D.transform;
											playgroundParticles.manipulators[i].SendParticleEventCollision();
										}
									}
								}
								if (playgroundParticles.hasEventManipulatorGlobal) {
									for (int i = 0; i<PlaygroundC.reference.manipulators.Count; i++) {
										if (PlaygroundC.reference.manipulators[i].trackParticles &&
										    PlaygroundC.reference.manipulators[i].sendEventCollision &&
											PlaygroundC.reference.manipulators[i].Contains (playgroundParticles.playgroundCache.position[p], PlaygroundC.reference.manipulators[i].transform.position)) {

											playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[i].manipulatorEventParticle, p);
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionCollider2D = hitInfo2D.collider;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionParticlePosition = hitInfo2D.point;
											playgroundParticles.manipulators[i].manipulatorEventParticle.collisionTransform = hitInfo2D.transform;
											PlaygroundC.reference.manipulators[i].SendParticleEventCollision();
										}
									}
								}
							}
						}
					} else {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
					}
				}
			}
		}

		// Returns the offset as a remainder from a point, constructed for threading
		public static Vector3 GetOverflow2 (Vector3 overflow, int currentVal, int maxVal) {
			float iteration = (currentVal/maxVal);
			return new Vector3(
				overflow.x*iteration,
				overflow.y*iteration,
				overflow.z*iteration
			);
		}

		// Returns the offset with direction as a remainder from a point, constructed for threading
		public static Vector3 GetOverflow2 (Vector3 overflow, Vector3 direction, int currentVal, int maxVal) {
			float iteration = (currentVal/maxVal);
			return new Vector3(
				direction.x*overflow.x*iteration,
				direction.y*overflow.y*iteration,
			 	direction.z*overflow.z*iteration
			);
		}

		public static Vector3 Vector3Scale (Vector3 v1, Vector3 v2) {
			return new Vector3(v1.x*v2.x,v1.y*v2.y,v1.z*v2.z);
		}

		// Calculate the effect from a manipulator
		public static void CalculateManipulator (PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, int p, float t, float life, Vector3 particlePosition, Vector3 manipulatorPosition, float manipulatorDistance, bool localSpace) {
			if (thisManipulator.enabled && thisManipulator.transform.available && thisManipulator.strength!=0 && thisManipulator.willAffect && thisManipulator.LifetimeFilter(life, playgroundParticles.lifetime) && thisManipulator.ParticleFilter (p, playgroundParticles.particleCount)) {

				bool contains = thisManipulator.Contains(localSpace?(playgroundParticles.particleSystemInverseRotation*particlePosition):particlePosition, localSpace?(playgroundParticles.particleSystemInverseRotation*manipulatorPosition):manipulatorPosition);

				// Is this a particle which shouldn't be affected by this manipulator?
				if (contains && (playgroundParticles.playgroundCache.excludeFromManipulatorId[p]==thisManipulator.manipulatorId || thisManipulator.nonAffectedParticles.Count>0 && thisManipulator.ContainsNonAffectedParticle(playgroundParticles.particleSystemId, p))) {
					return;
				}
				
				// Manipulator events
				if (thisManipulator.trackParticles) {

					// Particle entering
					if (contains) {
						if (!thisManipulator.ContainsParticle(playgroundParticles.particleSystemId, p)) {
							playgroundParticles.playgroundCache.manipulatorId[p] = thisManipulator.manipulatorId;
							thisManipulator.AddParticle(playgroundParticles.particleSystemId, p);
							if (thisManipulator.sendEventEnter) {
								playgroundParticles.UpdateEventParticle(thisManipulator.manipulatorEventParticle, p);
								thisManipulator.SendParticleEventEnter();
							}
						}
					} else {

						// Particle exiting
						if (thisManipulator.ContainsParticle(playgroundParticles.particleSystemId, p)) {
							playgroundParticles.playgroundCache.manipulatorId[p] = 0;
							thisManipulator.RemoveParticle(playgroundParticles.particleSystemId, p);
							if (thisManipulator.sendEventExit) {
								playgroundParticles.UpdateEventParticle(thisManipulator.manipulatorEventParticle, p);
								thisManipulator.SendParticleEventExit();
							}
						}
					}
				}

				if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.playgroundCache.noForce[p]) {
					// Attractors
					if (thisManipulator.type==MANIPULATORTYPEC.Attractor) {
						if (contains) {
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						}
					} else
						
					// Attractors Gravitational
					if (thisManipulator.type==MANIPULATORTYPEC.AttractorGravitational) {
						if (contains) {
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*thisManipulator.strength/manipulatorDistance, t/thisManipulator.strengthSmoothing);
						}
					} else
						
					// Repellents 
					if (thisManipulator.type==MANIPULATORTYPEC.Repellent) {
						if (contains) {
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*(thisManipulator.strength/manipulatorDistance), t*(thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						}
					} else

					// Vortex
					if (thisManipulator.type==MANIPULATORTYPEC.Vortex) {
						if (contains) {
							playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], ((manipulatorPosition-particlePosition)*thisManipulator.strength/manipulatorDistance)-Vector3.Cross(thisManipulator.transform.up, (manipulatorPosition-particlePosition))*thisManipulator.strength/manipulatorDistance, (t*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						}
					}
				}
				
				// Properties
				if (thisManipulator.type==MANIPULATORTYPEC.Property) {
					PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.property, p, t, particlePosition, manipulatorPosition, manipulatorDistance, localSpace, contains);
				} else
				
				// Combined
				if (thisManipulator.type==MANIPULATORTYPEC.Combined) {
					for (int i = 0; i<thisManipulator.properties.Count; i++)
						PropertyManipulator(playgroundParticles, thisManipulator, thisManipulator.properties[i], p, t, particlePosition, manipulatorPosition, manipulatorDistance, localSpace, contains);
				}
			}
		}
		
		// Calculate the effect from manipulator properties
		public static void PropertyManipulator (PlaygroundParticlesC playgroundParticles, ManipulatorObjectC thisManipulator, ManipulatorPropertyC thisManipulatorProperty, int p, float t, Vector3 particlePosition, Vector3 manipulatorPosition, float manipulatorDistance, bool localSpace, bool contains) {
			if (contains) {
				switch (thisManipulatorProperty.type) {
					
					// Velocity Property
					case MANIPULATORPROPERTYTYPEC.Velocity:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
						playgroundParticles.playgroundCache.velocity[p] = thisManipulatorProperty.useLocalRotation?
							thisManipulatorProperty.localVelocity
							:
							thisManipulatorProperty.velocity;
					else
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], thisManipulatorProperty.useLocalRotation?
							thisManipulatorProperty.localVelocity
							:
						thisManipulatorProperty.velocity, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
					break;
					
					// Additive Velocity Property
					case MANIPULATORPROPERTYTYPEC.AdditiveVelocity:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
						playgroundParticles.playgroundCache.velocity[p] += thisManipulatorProperty.useLocalRotation?
							thisManipulatorProperty.localVelocity*(t*thisManipulatorProperty.strength*thisManipulator.strength)
							:
							thisManipulatorProperty.velocity*(t*thisManipulatorProperty.strength*thisManipulator.strength);
					else
						playgroundParticles.playgroundCache.velocity[p] += Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], thisManipulatorProperty.useLocalRotation?
						    thisManipulatorProperty.localVelocity 
						    : 
						    thisManipulatorProperty.velocity,
						(t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
					break;
					
					// Color Property
					case MANIPULATORPROPERTYTYPEC.Color:
					Color staticColor;
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
						if (thisManipulatorProperty.keepColorAlphas) {
							staticColor = thisManipulatorProperty.color;
							staticColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, staticColor.a);
							playgroundParticles.particleCache[p].color = staticColor;
						} else playgroundParticles.particleCache[p].color = thisManipulatorProperty.color;
					} else {
						if (thisManipulatorProperty.keepColorAlphas) {
							staticColor = thisManipulatorProperty.color;
							staticColor.a = Mathf.Clamp(playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime).a, 0, staticColor.a);
							playgroundParticles.particleCache[p].color = Color.Lerp(playgroundParticles.particleCache[p].color, staticColor, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						} else playgroundParticles.particleCache[p].color = Color.Lerp(playgroundParticles.particleCache[p].color, thisManipulatorProperty.color, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
					}
					
					// Only color in range of manipulator boundaries
					if (!thisManipulatorProperty.onlyColorInRange)
						playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;

					// Keep alpha of original color
					if (thisManipulatorProperty.keepColorAlphas)
						playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = true;
					
					// Set color pairing key
					if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.manipulatorId) {
						playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.manipulatorId;
					}
					break;
					
					// Lifetime Color Property
					case MANIPULATORPROPERTYTYPEC.LifetimeColor:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
						playgroundParticles.particleCache[p].color = thisManipulatorProperty.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0);
					} else {
						playgroundParticles.particleCache[p].color = Color.Lerp(playgroundParticles.particleCache[p].color, thisManipulatorProperty.lifetimeColor.Evaluate(playgroundParticles.lifetime>0?playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime:0), (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
						playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = true;
					}
					
					// Only color in range of manipulator boundaries
					if (!thisManipulatorProperty.onlyColorInRange)
						playgroundParticles.playgroundCache.changedByPropertyColor[p] = true;
					
					// Set color pairing key
					else if (playgroundParticles.playgroundCache.propertyColorId[p] != thisManipulator.manipulatorId) {
						playgroundParticles.playgroundCache.propertyColorId[p] = thisManipulator.manipulatorId;
					}
					break;
					
					// Size Property
					case MANIPULATORPROPERTYTYPEC.Size:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
						playgroundParticles.playgroundCache.size[p] = thisManipulatorProperty.size;
					else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
						playgroundParticles.playgroundCache.size[p] = Mathf.Lerp(playgroundParticles.playgroundCache.size[p], thisManipulatorProperty.size, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
					else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
						playgroundParticles.playgroundCache.size[p] = Mathf.MoveTowards(playgroundParticles.playgroundCache.size[p], thisManipulatorProperty.size, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
					if (!thisManipulatorProperty.onlySizeInRange)
						playgroundParticles.playgroundCache.changedByPropertySize[p] = true;
					break;
					
					// Target Property
					case MANIPULATORPROPERTYTYPEC.Target:
					if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[thisManipulatorProperty.targetPointer].available) {
						
						
						// Set target pointer
						if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId) {

							playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetPointer;
							thisManipulatorProperty.targetPointer++; thisManipulatorProperty.targetPointer=thisManipulatorProperty.targetPointer%thisManipulatorProperty.targets.Count;
							playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
						}

						// Teleport or lerp to position based on transition type
						if (playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].available) {
							if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
								playgroundParticles.playgroundCache.position[p] = localSpace? 
									thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].localPosition
								: 
									thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position;
							else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
								playgroundParticles.playgroundCache.position[p] = localSpace? 
										Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].localPosition, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing)
								:
											Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
								playgroundParticles.playgroundCache.position[p] = localSpace?
										Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].localPosition, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing)
								:
											Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							}
							
							// This particle was changed by a target property
							playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
						}
					}
					break;
					
					// Death Property
					case MANIPULATORPROPERTYTYPEC.Death:
					if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None)
						playgroundParticles.playgroundCache.life[p] = playgroundParticles.lifetime;
					else
						playgroundParticles.playgroundCache.birth[p] -= (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing;
					
					// This particle was changed by a death property
					playgroundParticles.playgroundCache.changedByPropertyDeath[p] = true;
					break;
					
					
					// Attractors
					case MANIPULATORPROPERTYTYPEC.Attractor:
					if (!playgroundParticles.onlySourcePositioning) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance)/thisManipulator.strengthSmoothing);
					}
					break;
					
					// Attractors Gravitational
					case MANIPULATORPROPERTYTYPEC.Gravitational:
					if (!playgroundParticles.onlySourcePositioning) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (manipulatorPosition-particlePosition)*(thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance, t/thisManipulator.strengthSmoothing);
					}
					break;
					
					// Repellents 
					case MANIPULATORPROPERTYTYPEC.Repellent:
					if (!playgroundParticles.onlySourcePositioning) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], (particlePosition-manipulatorPosition)*((thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance), t*((thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance)/thisManipulator.strengthSmoothing);
					}
					break;

					// Vortex
					case MANIPULATORPROPERTYTYPEC.Vortex:
					if (!playgroundParticles.onlySourcePositioning) {
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], ((manipulatorPosition-particlePosition)*(thisManipulator.strength*thisManipulatorProperty.strength)/manipulatorDistance)-Vector3.Cross(thisManipulator.transform.up, (manipulatorPosition-particlePosition))*(thisManipulator.strength*thisManipulatorProperty.strength)/manipulatorDistance, (t*(thisManipulator.strength*thisManipulatorProperty.strength)/manipulatorDistance)/thisManipulator.strengthSmoothing);
					}
					break;

					// Turbulence
					case MANIPULATORPROPERTYTYPEC.Turbulence:
					if (!playgroundParticles.onlySourcePositioning && thisManipulatorProperty.turbulenceType!=TURBULENCETYPE.None) {
						Turbulence (
							playgroundParticles,
							thisManipulatorProperty.turbulenceSimplex,
							p,
							t/thisManipulator.strengthSmoothing, 
							thisManipulatorProperty.turbulenceType, 
							thisManipulatorProperty.turbulenceTimeScale, 
							thisManipulatorProperty.turbulenceScale,
							((thisManipulatorProperty.strength*thisManipulator.strength)/manipulatorDistance),
							thisManipulatorProperty.turbulenceApplyLifetimeStrength,
							thisManipulatorProperty.turbulenceLifetimeStrength
						);
					}
					break;

					// Mesh Target
					case MANIPULATORPROPERTYTYPEC.MeshTarget:
					if (!playgroundParticles.onlySourcePositioning && thisManipulatorProperty.meshTarget.initialized) {

						// Set target pointer
						if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId) {
							playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetSortingList[thisManipulatorProperty.targetPointer];
							thisManipulatorProperty.targetPointer++; thisManipulatorProperty.targetPointer=thisManipulatorProperty.targetPointer%thisManipulatorProperty.meshTarget.vertexPositions.Length;
							playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
						}

						// Teleport or lerp to position based on transition type
						if (playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId) {
							if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
								playgroundParticles.playgroundCache.position[p] = thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.meshTarget.vertexPositions.Length]);
								playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
									playgroundParticles.playgroundCache.position[p] = Vector3.Lerp(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.meshTarget.vertexPositions.Length]), (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
									playgroundParticles.playgroundCache.position[p] = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.meshTarget.vertexPositions.Length]), (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							}
							
							// This particle was changed by a target property
							playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
						}

					}
					break;
				
					// Skinned Mesh Target
					case MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget:
					if (!playgroundParticles.onlySourcePositioning && thisManipulatorProperty.skinnedMeshTarget.initialized) {
							
						// Set target pointer
						if (playgroundParticles.playgroundCache.propertyId[p] != thisManipulator.manipulatorId) {
							playgroundParticles.playgroundCache.propertyTarget[p] = thisManipulatorProperty.targetSortingList[thisManipulatorProperty.targetPointer];
							thisManipulatorProperty.targetPointer++; thisManipulatorProperty.targetPointer=thisManipulatorProperty.targetPointer%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length;
							playgroundParticles.playgroundCache.propertyId[p] = thisManipulator.manipulatorId;
						}

						// Teleport or lerp to position based on transition type
						if (playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId) {
							if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.None) {
								playgroundParticles.playgroundCache.position[p] = thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length];
								playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
									playgroundParticles.playgroundCache.position[p] = Vector3.Lerp(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t*(thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
									playgroundParticles.playgroundCache.position[p] = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t*(thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							}
							
							// This particle was changed by a target property
							playgroundParticles.playgroundCache.changedByPropertyTarget[p] = true;
						}
					}
					break;
				}
				
				playgroundParticles.playgroundCache.changedByProperty[p] = true;
				
			} else {
				
				// Handle colors outside of property manipulator range
				if (playgroundParticles.playgroundCache.propertyColorId[p] == thisManipulator.manipulatorId && (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Color || thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.LifetimeColor)) {

					// Lerp back color with previous set key
					if (playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITIONC.None && thisManipulatorProperty.onlyColorInRange) {
						playgroundParticles.particleCache[p].color = Color.Lerp(playgroundParticles.particleCache[p].color, playgroundParticles.lifetimeColor.Evaluate(playgroundParticles.playgroundCache.life[p]/playgroundParticles.lifetime), t*thisManipulatorProperty.strength*thisManipulator.strength);
					}
				}

				// Position onto targets when outside of range
				if (!playgroundParticles.onlySourcePositioning && !thisManipulatorProperty.onlyPositionInRange && thisManipulatorProperty.transition != MANIPULATORPROPERTYTRANSITIONC.None) {

					// Target positioning outside of range
					if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.Target) {
						if (thisManipulatorProperty.targets.Count>0 && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count]!=null) {
							if (playgroundParticles.playgroundCache.changedByPropertyTarget[p] && thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].available && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.transform.GetInstanceID()) {
								if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp)
									playgroundParticles.playgroundCache.position[p] = localSpace?
										Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].localPosition, t*(thisManipulatorProperty.strength*thisManipulator.strength)/thisManipulator.strengthSmoothing)
										:	
										Vector3.Lerp(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*(thisManipulatorProperty.strength*thisManipulator.strength)/thisManipulator.strengthSmoothing);
								else
									playgroundParticles.playgroundCache.position[p] = localSpace?
										Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].localPosition, t*(thisManipulatorProperty.strength*thisManipulator.strength)/thisManipulator.strengthSmoothing)
										:
										Vector3.MoveTowards(particlePosition, thisManipulatorProperty.targets[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.targets.Count].position, t*(thisManipulatorProperty.strength*thisManipulator.strength)/thisManipulator.strengthSmoothing);
								
								if (thisManipulatorProperty.zeroVelocityStrength>0)
									playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
							}
						}
					}

					// Mesh Target positioning outside of range
					if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.MeshTarget && thisManipulatorProperty.meshTarget.initialized && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId) {
						if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
							playgroundParticles.playgroundCache.position[p] = Vector3.Lerp(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.meshTarget.vertexPositions.Length]), (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
							playgroundParticles.playgroundCache.position[p] = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.meshTargetMatrix.MultiplyPoint3x4(thisManipulatorProperty.meshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.meshTarget.vertexPositions.Length]), (t*thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						}
					}

					// Skinned Mesh Target positioning outside of range
					if (thisManipulatorProperty.type == MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget && thisManipulatorProperty.skinnedMeshTarget.initialized && playgroundParticles.playgroundCache.propertyId[p] == thisManipulator.manipulatorId) {
						if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Lerp) {
							playgroundParticles.playgroundCache.position[p] = Vector3.Lerp(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t*(thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						} else if (thisManipulatorProperty.transition == MANIPULATORPROPERTYTRANSITIONC.Linear) {
							playgroundParticles.playgroundCache.position[p] = Vector3.MoveTowards(particlePosition, thisManipulatorProperty.skinnedMeshTarget.vertexPositions[playgroundParticles.playgroundCache.propertyTarget[p]%thisManipulatorProperty.skinnedMeshTarget.vertexPositions.Length], t*(thisManipulatorProperty.strength*thisManipulator.strength/manipulatorDistance)/thisManipulator.strengthSmoothing);
							if (thisManipulatorProperty.zeroVelocityStrength>0)
								playgroundParticles.playgroundCache.velocity[p] = Vector3.Lerp(playgroundParticles.playgroundCache.velocity[p], Vector3.zero, t*thisManipulatorProperty.zeroVelocityStrength);
						}
					}
				}
			}
		}
		
		// Update the source scatter
		public void RefreshScatter () {
			System.Random random = new System.Random();
			for (int p = 0; p<particleCount; p++) {
				playgroundCache.scatterPosition[p] = applySourceScatter?RandomRange(random, sourceScatterMin, sourceScatterMax) : Vector3.zero;
			}
		}

		// Random range float adapted for threading
		public static float RandomRange (System.Random random, float min, float max) {
			return min+((float)random.NextDouble())*(max-min);
		}

		// Random range Vector3 adapted for threading
		public static Vector3 RandomRange (System.Random random, Vector3 min, Vector3 max) {
			return new Vector3 (
				RandomRange(random, min.x, max.x),
				RandomRange(random, min.y, max.y),
				RandomRange(random, min.z, max.z)
			);
		}

		// Return a random float array
		public static float[] RandomFloat (int length, float min, float max, System.Random random) {
			float[] f = new float[length];
			for (int i = 0; i<length; i++) {
				f[i] = RandomRange (random, min, max);
			}
			return f;
		}

		public void InactivateParticle (int particleId) {
			playgroundCache.simulate[particleId] = false;
			playgroundCache.rebirth[particleId] = false;
			playgroundCache.isNonBirthed[particleId] = true;
			playgroundCache.isFirstLoop[particleId] = true;
			playgroundCache.position[particleId] = PlaygroundC.initialTargetPosition;
			playgroundCache.targetPosition[particleId] = playgroundCache.position[particleId];
			playgroundCache.previousTargetPosition[particleId] = playgroundCache.targetPosition[particleId];
			playgroundCache.velocity[particleId] = Vector3.zero;
			particleCache[particleId].position = playgroundCache.position[particleId];
			particleCache[particleId].size = 0;

			// Reset manipulators influence
			playgroundCache.changedByProperty[particleId] = false;
			playgroundCache.changedByPropertyColor[particleId] = false;
			playgroundCache.changedByPropertyColorLerp[particleId] = false;
			playgroundCache.changedByPropertyColorKeepAlpha[particleId] = false;
			playgroundCache.changedByPropertySize[particleId] = false;
			playgroundCache.changedByPropertyTarget[particleId] = false;
			playgroundCache.changedByPropertyDeath[particleId] = false;
			playgroundCache.propertyTarget[particleId] = 0;
			playgroundCache.propertyId[particleId] = 0;
			playgroundCache.propertyColorId[particleId] = 0;
			playgroundCache.manipulatorId[particleId] = 0;
		}

		public void InactivateParticles () {
			for (int p = 0; p<particleCount; p++)
				InactivateParticle(p);

			hasActiveParticles = false;
			threadHadNoActiveParticles = true;
			isDoneThread = true;
			if (particleCache!=null && particleCache.Length>0)
				shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
		}

		// Rebirth of a specified particle
		public static void Rebirth (PlaygroundParticlesC playgroundParticles, int p, System.Random random) {
			if (!playgroundParticles.hasActiveParticles) return;
			// Set initial values
			playgroundParticles.playgroundCache.rebirth[p] = playgroundParticles.source==SOURCEC.Script?true:(playgroundParticles.emit && (playgroundParticles.loop || playgroundParticles.playgroundCache.isNonBirthed[p]) && playgroundParticles.playgroundCache.emission[p]);
			playgroundParticles.playgroundCache.isFirstLoop[p] = playgroundParticles.playgroundCache.isNonBirthed[p];
			playgroundParticles.playgroundCache.isNonBirthed[p] = false;
			playgroundParticles.playgroundCache.birthDelay[p] = 0f;
			playgroundParticles.playgroundCache.life[p] = 0f;
			playgroundParticles.playgroundCache.birth[p] = playgroundParticles.playgroundCache.death[p];
			playgroundParticles.playgroundCache.death[p] += playgroundParticles.lifetime;
			playgroundParticles.playgroundCache.velocity[p] = Vector3.zero;
			playgroundParticles.playgroundCache.noForce[p] = false;

			// Set new random lifetime
			if (playgroundParticles.applyRandomLifetimeOnRebirth) {
				if (playgroundParticles.lifetimeValueMethod==VALUEMETHOD.Constant) {
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = 0;
				} else {
					playgroundParticles.playgroundCache.lifetimeSubtraction[p] = playgroundParticles.lifetime-RandomRange (random, playgroundParticles.lifetimeMin, playgroundParticles.lifetime);
				}
			}

			// Set new random size
			if (playgroundParticles.applyRandomSizeOnRebirth)
				playgroundParticles.playgroundCache.initialSize[p] = RandomRange(random, playgroundParticles.sizeMin, playgroundParticles.sizeMax);

			// Initial velocity
			if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning) {
						
				// Initial global velocity
				if (playgroundParticles.applyInitialVelocity) {
					if (playgroundParticles.applyRandomInitialVelocityOnRebirth)
						playgroundParticles.playgroundCache.initialVelocity[p] = RandomRange(random, playgroundParticles.initialVelocityMin, playgroundParticles.initialVelocityMax);
					playgroundParticles.playgroundCache.velocity[p] = playgroundParticles.playgroundCache.initialVelocity[p];
					
					// Give this spawning particle its velocity shape
					if (playgroundParticles.applyInitialVelocityShape)
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f), playgroundParticles.initialVelocityShapeScale));
				}

				// Initial local velocity
				if (playgroundParticles.applyInitialLocalVelocity && playgroundParticles.source!=SOURCEC.Script) {
					playgroundParticles.playgroundCache.initialLocalVelocity[p] = RandomRange(random, playgroundParticles.initialLocalVelocityMin, playgroundParticles.initialLocalVelocityMax);
					playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.playgroundCache.targetDirection[p];
					
					// Give this spawning particle its local velocity shape
					if (playgroundParticles.applyInitialVelocityShape)
						playgroundParticles.playgroundCache.velocity[p] = Vector3.Scale(playgroundParticles.playgroundCache.velocity[p], playgroundParticles.initialVelocityShape.Evaluate((p*1f)/(playgroundParticles.particleCount*1f), playgroundParticles.initialVelocityShapeScale));
				}

				// Initial stretch
				if (playgroundParticles.renderModeStretch) {
					if (playgroundParticles.playgroundCache.velocity[p]!=Vector3.zero)
						playgroundParticles.particleCache[p].velocity = playgroundParticles.playgroundCache.velocity[p];
					else 
						playgroundParticles.particleCache[p].velocity = playgroundParticles.stretchStartDirection;
				} else
					playgroundParticles.particleCache[p].velocity = Vector3.zero;
			}
			if (playgroundParticles.source==SOURCEC.Script) {
				// Velocity for script mode
				if (!playgroundParticles.onlySourcePositioning && !playgroundParticles.onlyLifetimePositioning)
					playgroundParticles.playgroundCache.velocity[p] += playgroundParticles.scriptedEmissionVelocity;
				playgroundParticles.playgroundCache.targetPosition[p] = playgroundParticles.scriptedEmissionPosition;
				playgroundParticles.particleCache[p].velocity = playgroundParticles.playgroundCache.velocity[p];
			}
			
			if (playgroundParticles.playgroundCache.rebirth[p]) {

				// Source Scattering
				if (playgroundParticles.applySourceScatter && playgroundParticles.source!=SOURCEC.Script) {
					if (playgroundParticles.playgroundCache.scatterPosition[p]==Vector3.zero || playgroundParticles.applyRandomScatterOnRebirth)
						playgroundParticles.playgroundCache.scatterPosition[p] = RandomRange(random, playgroundParticles.sourceScatterMin, playgroundParticles.sourceScatterMax);
				} else playgroundParticles.playgroundCache.scatterPosition[p] = Vector3.zero;

				if (!playgroundParticles.onlyLifetimePositioning) {
					playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.targetPosition[p];
					playgroundParticles.particleCache[p].position = playgroundParticles.playgroundCache.targetPosition[p];
					playgroundParticles.playgroundCache.previousParticlePosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
					playgroundParticles.playgroundCache.collisionParticlePosition[p] = playgroundParticles.playgroundCache.targetPosition[p];
				} else if (!playgroundParticles.onlySourcePositioning) {
					Vector3 evalLifetimePosition = playgroundParticles.lifetimePositioning.Evaluate(0f, playgroundParticles.lifetimePositioningScale);
					if (playgroundParticles.lifetimePositioningUsesSourceDirection)
						evalLifetimePosition = playgroundParticles.playgroundCache.targetDirection[p];
					playgroundParticles.playgroundCache.position[p] = playgroundParticles.playgroundCache.targetPosition[p]+evalLifetimePosition;
					playgroundParticles.particleCache[p].position = playgroundParticles.playgroundCache.targetPosition[p]+evalLifetimePosition;
					playgroundParticles.playgroundCache.previousParticlePosition[p] = playgroundParticles.playgroundCache.targetPosition[p]+evalLifetimePosition;
					playgroundParticles.playgroundCache.collisionParticlePosition[p] = playgroundParticles.playgroundCache.targetPosition[p]+evalLifetimePosition;
				}

				if (playgroundParticles.applyInitialColorOnRebirth) {
					playgroundParticles.particleCache[p].color = playgroundParticles.playgroundCache.initialColor[p];
					playgroundParticles.playgroundCache.color[p] = playgroundParticles.playgroundCache.initialColor[p];
				}
			} else {
				playgroundParticles.particleCache[p].position = PlaygroundC.initialTargetPosition;
			}

			// Set new random rotation
			if (playgroundParticles.applyRandomRotationOnRebirth && !playgroundParticles.rotateTowardsDirection)
				playgroundParticles.playgroundCache.initialRotation[p] = RandomRange(random, playgroundParticles.initialRotationMin, playgroundParticles.initialRotationMax);
			
			if (!playgroundParticles.rotateTowardsDirection)
				playgroundParticles.playgroundCache.rotation[p] = playgroundParticles.playgroundCache.initialRotation[p];
			else {
				Vector3 particleDir;
				if (!playgroundParticles.onlySourcePositioning&&playgroundParticles.onlyLifetimePositioning)
					particleDir = (playgroundParticles.playgroundCache.position[p]+playgroundParticles.lifetimePositioning.Evaluate(.01f, playgroundParticles.lifetimePositioningScale))-playgroundParticles.playgroundCache.position[p];
				else
					particleDir = playgroundParticles.playgroundCache.velocity[p];
				playgroundParticles.playgroundCache.rotation[p] = playgroundParticles.playgroundCache.initialRotation[p]+SignedAngle(
					Vector3.up,
					particleDir,
					playgroundParticles.rotationNormal
				);
			}

			playgroundParticles.particleCache[p].rotation = playgroundParticles.playgroundCache.rotation[p];

			// Set size
			if (playgroundParticles.applyLifetimeSize) {
				//playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p]*playgroundParticles.lifetimeSize.Evaluate(0f)*playgroundParticles.scale;
			} else
				playgroundParticles.playgroundCache.size[p] = playgroundParticles.playgroundCache.initialSize[p]*playgroundParticles.scale;
			playgroundParticles.particleCache[p].size = p>=playgroundParticles.particleMask?playgroundParticles.playgroundCache.size[p]:0;

			// Set color gradient id
			if (playgroundParticles.colorSource==COLORSOURCEC.LifetimeColors && playgroundParticles.lifetimeColors.Count>0) {
				playgroundParticles.lifetimeColorId++;playgroundParticles.lifetimeColorId=playgroundParticles.lifetimeColorId%playgroundParticles.lifetimeColors.Count;
				playgroundParticles.playgroundCache.lifetimeColorId[p] = playgroundParticles.lifetimeColorId;
			}

			// Reset manipulators influence
			playgroundParticles.playgroundCache.changedByProperty[p] = false;
			playgroundParticles.playgroundCache.changedByPropertyColor[p] = false;
			playgroundParticles.playgroundCache.changedByPropertyColorLerp[p] = false;
			playgroundParticles.playgroundCache.changedByPropertyColorKeepAlpha[p] = false;
			playgroundParticles.playgroundCache.changedByPropertySize[p] = false;
			playgroundParticles.playgroundCache.changedByPropertyTarget[p] = false;
			playgroundParticles.playgroundCache.changedByPropertyDeath[p] = false;
			playgroundParticles.playgroundCache.propertyTarget[p] = 0;
			playgroundParticles.playgroundCache.propertyId[p] = 0;
			playgroundParticles.playgroundCache.propertyColorId[p] = 0;
			playgroundParticles.playgroundCache.manipulatorId[p] = 0;

			// Send birth event
			if (playgroundParticles.events.Count>0 && playgroundParticles.playgroundCache.rebirth[p])
				playgroundParticles.SendEvent(EVENTTYPEC.Birth, p);
			if (playgroundParticles.hasEventManipulatorLocal) {
				for (int i = 0; i<playgroundParticles.manipulators.Count; i++) {
					if (playgroundParticles.manipulators[i].trackParticles &&
					    playgroundParticles.manipulators[i].sendEventBirth &&
						playgroundParticles.manipulators[i].Contains (playgroundParticles.playgroundCache.targetPosition[p], playgroundParticles.manipulators[i].transform.position)) {

						playgroundParticles.UpdateEventParticle(playgroundParticles.manipulators[i].manipulatorEventParticle, p);
						playgroundParticles.manipulators[i].SendParticleEventBirth();
					}
				}
			}
			if (playgroundParticles.hasEventManipulatorGlobal) {
				for (int i = 0; i<PlaygroundC.reference.manipulators.Count; i++) {
					if (PlaygroundC.reference.manipulators[i].trackParticles &&
					    PlaygroundC.reference.manipulators[i].sendEventBirth &&
						PlaygroundC.reference.manipulators[i].Contains (playgroundParticles.playgroundCache.targetPosition[p], PlaygroundC.reference.manipulators[i].transform.position)) {

						playgroundParticles.UpdateEventParticle(PlaygroundC.reference.manipulators[i].manipulatorEventParticle, p);
						PlaygroundC.reference.manipulators[i].SendParticleEventBirth();
					}
				}
			}
		}
		
		// Sends an event to Event- Targets and Listeners
		void SendEvent (EVENTTYPEC eventType, int p) {
			SendEvent(eventType, p, playgroundCache.velocity[p], null, null, null);
		}
		void SendEvent (EVENTTYPEC eventType, int p, Vector3 preEventVelocity) {
			SendEvent(eventType, p, preEventVelocity, null, null, null);
		}
		void SendEvent (EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform) {
			SendEvent(eventType, p, preEventVelocity, collisionTransform, null, null);
		}
		void SendEvent (EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider collisionCollider) {
			SendEvent(eventType, p, preEventVelocity, collisionTransform, collisionCollider, null);
		}
		void SendEvent (EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider2D collisionCollider2D) {
			SendEvent(eventType, p, preEventVelocity, collisionTransform, null, collisionCollider2D);
		}
		void SendEvent (EVENTTYPEC eventType, int p, Vector3 preEventVelocity, Transform collisionTransform, Collider collisionCollider, Collider2D collisionCollider2D) {
			Vector3 eventPosition = Vector3.zero;
			Vector3 eventVelocity = Vector3.zero;
			Color32 eventColor = Color.white;

			// Loop through available events
			for (int i = 0; i<events.Count; i++) {
				if (events[i].enabled && events[i].eventType==eventType) {

					// Check thresholds
					if (events[i].eventType==EVENTTYPEC.Collision)
						if (events[i].collisionThreshold>preEventVelocity.sqrMagnitude)
							return;
					
					// Set event position
					switch (events[i].eventInheritancePosition) {
						case EVENTINHERITANCEC.User:
						eventPosition = events[i].eventPosition;
						break;
						case EVENTINHERITANCEC.Particle:
						eventPosition = playgroundCache.position[p];
						break;
						case EVENTINHERITANCEC.Source:
						eventPosition = playgroundCache.targetPosition[p];
						break;
					}

					// Set event velocity
					switch (events[i].eventInheritanceVelocity) {
						case EVENTINHERITANCEC.User:
						eventVelocity = events[i].eventVelocity;
						break;
						case EVENTINHERITANCEC.Particle:
						eventVelocity = playgroundCache.velocity[p];
						break;
						case EVENTINHERITANCEC.Source:
						if (applyInitialLocalVelocity)
							eventVelocity = playgroundCache.initialLocalVelocity[p];
						if (applyInitialVelocity)
							eventVelocity += playgroundCache.initialVelocity[p];
						if (applyLifetimeVelocity)
							eventVelocity += lifetimeVelocity.Evaluate(Mathf.Clamp01(playgroundCache.life[p]/lifetime), lifetimeVelocityScale);
						if (applyInitialVelocityShape)
							eventVelocity = Vector3.Scale(eventVelocity, initialVelocityShape.Evaluate((p*1f)/(particleCount*1f), initialVelocityShapeScale));
						break;
					}

					// Apply multiplier
					eventVelocity *= events[i].velocityMultiplier;

					// Set event color
					switch (events[i].eventInheritanceColor) {
						case EVENTINHERITANCEC.User:
						eventColor = events[i].eventColor;
						break;
						case EVENTINHERITANCEC.Particle:
						eventColor = particleCache[p].color;
						break;
						case EVENTINHERITANCEC.Source:
						eventColor = playgroundCache.initialColor[p];
						break;
					}

					// Send the event to any Event Listeners
					if (events[i].broadcastType==EVENTBROADCASTC.EventListeners || events[i].broadcastType==EVENTBROADCASTC.Both) {

						UpdateEventParticle(eventParticle, p);
						eventParticle.collisionCollider = collisionCollider;
						eventParticle.collisionCollider2D = collisionCollider2D;
						eventParticle.collisionTransform = collisionTransform;

						events[i].SendParticleEvent(eventParticle);

						// Send the event to the Playground Manager
						if (events[i].sendToManager) {
							switch (events[i].eventType) {
								case EVENTTYPEC.Birth:
								PlaygroundC.SendParticleEventBirth(eventParticle);
								break;
								case EVENTTYPEC.Death:
								PlaygroundC.SendParticleEventDeath(eventParticle);
								break;
								case EVENTTYPEC.Collision:
								PlaygroundC.SendParticleEventCollision(eventParticle);
								break;
								case EVENTTYPEC.Time:
								PlaygroundC.SendParticleEventTime(eventParticle);
								break;
							}
						}
					}

					// Send the event to target
					if (events[i].initializedTarget && (events[i].broadcastType==EVENTBROADCASTC.Target || events[i].broadcastType==EVENTBROADCASTC.Both)) {
						events[i].target.ThreadSafeEmit(eventPosition, eventVelocity, eventColor);
					}
				}
			}
		}

		public void UpdateEventParticle (PlaygroundEventParticle eParticle, int p) {
			eParticle.particleSystemId = particleSystemId;
			eParticle.particleId = p;
			eParticle.birth = playgroundCache.birth[p];
			eParticle.birthDelay = playgroundCache.birthDelay[p];
			eParticle.changedByProperty = playgroundCache.changedByProperty[p];
			eParticle.changedByPropertyColor = playgroundCache.changedByPropertyColor[p];
			eParticle.changedByPropertyColorKeepAlpha = playgroundCache.changedByPropertyColorKeepAlpha[p];
			eParticle.changedByPropertyColorLerp = playgroundCache.changedByPropertyColorLerp[p];
			eParticle.changedByPropertyDeath = playgroundCache.changedByPropertyDeath[p];
			eParticle.changedByPropertySize = playgroundCache.changedByPropertySize[p];
			eParticle.changedByPropertyTarget = playgroundCache.changedByPropertyTarget[p];
			eParticle.collisionParticlePosition = playgroundCache.collisionParticlePosition[p];
			eParticle.color = particleCache[p].color;
			eParticle.scriptedColor = playgroundCache.scriptedColor[p];
			eParticle.death = playgroundCache.death[p];
			eParticle.emission = playgroundCache.emission[p];
			eParticle.initialColor = playgroundCache.initialColor[p];
			eParticle.initialLocalVelocity = playgroundCache.initialLocalVelocity[p];
			eParticle.initialRotation = playgroundCache.initialRotation[p];
			eParticle.initialSize = playgroundCache.initialSize[p];
			eParticle.initialVelocity = playgroundCache.initialVelocity[p];
			eParticle.initialLocalVelocity = playgroundCache.initialLocalVelocity[p];
			eParticle.life = playgroundCache.life[p];
			eParticle.lifetimeColorId = playgroundCache.lifetimeColorId[p];
			eParticle.noForce = playgroundCache.noForce[p];
			eParticle.lifetimeOffset = playgroundCache.lifetimeOffset[p];
			eParticle.localSpaceMovementCompensation = playgroundCache.localSpaceMovementCompensation[p];
			eParticle.position = playgroundCache.position[p];
			eParticle.previousParticlePosition = playgroundCache.previousParticlePosition[p];
			eParticle.previousTargetPosition = playgroundCache.previousTargetPosition[p];
			eParticle.propertyColorId = playgroundCache.propertyColorId[p];
			eParticle.propertyId = playgroundCache.propertyId[p];
			eParticle.excludeFromManipulatorId = playgroundCache.excludeFromManipulatorId[p];
			eParticle.propertyTarget = playgroundCache.propertyTarget[p];
			eParticle.rebirth = playgroundCache.rebirth[p];
			eParticle.rotation = playgroundCache.rotation[p];
			eParticle.rotationSpeed = playgroundCache.rotationSpeed[p];
			eParticle.scatterPosition = playgroundCache.scatterPosition[p];
			eParticle.size = playgroundCache.size[p];
			eParticle.targetDirection = playgroundCache.targetDirection[p];
			eParticle.targetPosition = playgroundCache.targetPosition[p];
			eParticle.velocity = playgroundCache.velocity[p];
			eParticle.isMasked = playgroundCache.isMasked[p];
			eParticle.maskAlpha = playgroundCache.maskAlpha[p];
			eParticle.isFirstLoop = playgroundCache.isFirstLoop[p];
			eParticle.isNonBirthed = playgroundCache.isNonBirthed[p];
		}
		
		// Delete a state from states list
		public void RemoveState (int i) {
			int newState = activeState;
			newState = (newState%states.Count)-1;
			if (newState<0) newState = 0;
			
			states[newState].Initialize();
			activeState = newState;
			states.RemoveAt(i);
		}
		
		// Wipe out particles in current PlaygroundParticlesC object
		public static void Clear (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.inTransition = false;
			playgroundParticles.particleCache = new ParticleSystem.Particle[0];
			playgroundParticles.playgroundCache = null;
			playgroundParticles.shurikenParticleSystem.SetParticles(playgroundParticles.particleCache,0);
			playgroundParticles.shurikenParticleSystem.Clear();
		}

		// Store the current state a particle system is in
		public void Save () {
			if (isSnapshot) {
				Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
				return;
			}
			StartCoroutine (SaveRoutine ("New Snapshot "+(snapshots.Count+1).ToString()));
		}

		// Store the current state a particle system is in and name it
		public void Save (string saveName) {
			if (isSnapshot) {
				Debug.Log("A snapshot can't store snapshot data within itself.", gameObject);
				return;
			}
			StartCoroutine (SaveRoutine (saveName));
		}

		IEnumerator SaveRoutine (string saveName) {
			PlaygroundSave data = new PlaygroundSave();
			data.settings = PlaygroundC.Particle();
			data.settings.isSnapshot = true;
			yield return null;
			data.Save(this);
			data.settings.transform.parent = particleSystemTransform;
			data.settings.transform.name = saveName;
			data.settings.timeOfSnapshot = PlaygroundC.globalTime;
			data.name = saveName;
			data.time = PlaygroundC.globalTime;
			data.particleCount = particleCount;
			data.lifetime = lifetime;
			snapshots.Add (data);
			PlaygroundC.reference.particleSystems.Remove (data.settings);
			#if UNITY_EDITOR
			if (!PlaygroundC.reference.showSnapshotsInHierarchy)
				data.settings.gameObject.hideFlags = HideFlags.HideInHierarchy;
			#endif
		}

		// Load from a saved data state using an int
		public void Load (int loadPointer) {
			if (snapshots.Count>0) {
				loadPointer = loadPointer%snapshots.Count;
				StartCoroutine(LoadRoutine(loadPointer));
			} else {
				Debug.Log ("No data to load from. Please use PlaygroundParticlesC.Save() to store a particle system's current state.", particleSystemGameObject);
			}
		}

		// Load from a saved data state using a string
		public void Load (string loadName) {
			if (snapshots.Count>0) {
				for (int i = 0; i<snapshots.Count; i++) { 
					if (snapshots[i].name == loadName.Trim()) {
						StartCoroutine(LoadRoutine(i));
						return;
					}
				}
			} else {
				Debug.Log ("No data found with the name "+loadName+".", particleSystemGameObject);
			}
		}

		// Snapshot loading routine
		bool isLoading = false;
		bool transitionAvailable = false;
		IEnumerator LoadRoutine (int loadPointer) {
			if (loadTransition && loadTransitionTime>0 && snapshots[loadPointer].transitionMultiplier>0 && transitionAvailable && !isYieldRefreshing) {
				if (snapshots[loadPointer].loadMaterial && !snapshots[loadPointer].setMaterialAfterTransition && snapshots[loadPointer].settings.particleSystemRenderer.sharedMaterial!=null) 
					particleSystemRenderer.sharedMaterial = snapshots[loadPointer].settings.particleSystemRenderer.sharedMaterial;
				StartCoroutine(LoadTransition(loadPointer));
				while (inTransition) yield return null;
			}
			if (isLoading || inTransition || abortTransition)
				yield break;

			isLoading = true;
			int prevParticleCount = particleCount;
			if (prevParticleCount!=snapshots[loadPointer].settings.particleCount) {
				SetParticleCount(thisInstance, snapshots[loadPointer].settings.particleCount);
				while (isSettingParticleCount)
					yield return null;
			}
			snapshots[loadPointer].Load(thisInstance);
			while (snapshots[loadPointer].IsLoading())
				yield return null;
			lastTimeUpdated = PlaygroundC.globalTime;
			cameFromNonCalculatedFrame = false;
			cameFromNonEmissionFrame = false;
			//yield return null;
			if (snapshots[loadPointer].loadMode!=1) {
				if (loadFromStart && isYieldRefreshing)
					yield return null;
				float tos = snapshots[loadPointer].settings.timeOfSnapshot<=0?snapshots[loadPointer].time:snapshots[loadPointer].settings.timeOfSnapshot;

				for (int p = 0; p<particleCount; p++) {
					playgroundCache.birth[p] = PlaygroundC.globalTime+(playgroundCache.birth[p]-tos); 
					playgroundCache.death[p] = PlaygroundC.globalTime+(playgroundCache.death[p]-tos);
					playgroundCache.size[p] = snapshots[loadPointer].settings.snapshotData.size[p];
					particleCache[p].size = playgroundCache.size[p];
				}

				lifetime = snapshots[loadPointer].settings.lifetime;
				previousLifetimeValueMethod = lifetimeValueMethod;
				previousLifetime = lifetime;
				previousLifetimeMin = lifetimeMin;
				previousNearestNeighborOrigin = nearestNeighborOrigin;
				previousSorting = sorting;
				localDeltaTime = .001f;
			} else if (prevParticleCount!=particleCount) {
				SetLifetime(thisInstance, sorting, lifetime);
				yield return null;
				while (isSettingLifetime) {
					yield return null;
				}
			}
			initNearestNeighborSeq = 6;
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;
			lastTimeUpdated = PlaygroundC.globalTime;
			cameFromNonCalculatedFrame = false;
			cameFromNonEmissionFrame = false;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			isLoading = false;
			isDoneThread = true;
			transitionAvailable = true;
		}

		// Apply transition between snapshot load
		bool abortTransition = false;
		IEnumerator LoadTransition (int loadPointer) {
			if (inTransition) {
				abortTransition = true;
				yield return null;
			}
			abortTransition = false;
			inTransition = true;

			float transitionStartTime = PlaygroundC.globalTime;
			int loadParticleCount = snapshots[loadPointer].settings.particleCount;
			bool liveParticles = snapshots[loadPointer].loadMode!=1;
			bool firstFrameDone = false;
			int currentParticleCount = particleCount;
			int transitionParticleCount = currentParticleCount;
			PlaygroundCache loadSnapshotData = snapshots[loadPointer].settings.snapshotData;

			INDIVIDUALTRANSITIONTYPEC thisSnapshotTransition = snapshots[loadPointer].transitionType;
			if (thisSnapshotTransition==INDIVIDUALTRANSITIONTYPEC.Inherit) {
				thisSnapshotTransition = (INDIVIDUALTRANSITIONTYPEC)((int)loadTransitionType+1);
			}

			// Prepare arrays
			Vector3[] transitionPosition = (Vector3[])playgroundCache.position.Clone();
			Color32[] transitionColor = (Color32[])playgroundCache.color.Clone();
			float[] transitionSize = (float[])playgroundCache.size.Clone();
			float[] transitionRotation = (float[])playgroundCache.rotation.Clone();

			// Resize if more particles are needed
			if (loadParticleCount>particleCount) {
				PlaygroundC.RunAsync (()=>{
					System.Array.Resize(ref transitionPosition, loadParticleCount);
					System.Array.Resize(ref transitionColor, loadParticleCount);
					System.Array.Resize(ref transitionSize, loadParticleCount);
					System.Array.Resize(ref transitionRotation, loadParticleCount);
					System.Array.Resize(ref transitionRotation, loadParticleCount);

					System.Array.Resize(ref playgroundCache.position, loadParticleCount);
					System.Array.Resize(ref playgroundCache.color, loadParticleCount);
					System.Array.Resize(ref playgroundCache.size, loadParticleCount);
					System.Array.Resize(ref playgroundCache.rotation, loadParticleCount);

					for (int p = particleCount; p<loadParticleCount; p++) {
						transitionPosition[p] = transitionPosition[p%particleCount];
						transitionColor[p].a = 0;
					}
				});
				yield return null;
				particleCache = new ParticleSystem.Particle[loadParticleCount];
				shurikenParticleSystem.Emit(loadParticleCount);
				shurikenParticleSystem.GetParticles(particleCache);
				transitionParticleCount = loadParticleCount;
			}

			// Transition
			while (PlaygroundC.globalTime<transitionStartTime+((loadTransitionTime*snapshots[loadPointer].transitionMultiplier)-.001f) && inTransition && !abortTransition && loadTransition && (loadTransitionTime*snapshots[loadPointer].transitionMultiplier)>0 && currentParticleCount==particleCount) {
				float currentTime = PlaygroundC.globalTime;
				PlaygroundC.RunAsync (()=>{
					float t = TransitionType (thisSnapshotTransition, (currentTime-transitionStartTime)/(loadTransitionTime*snapshots[loadPointer].transitionMultiplier));
					for (int p = 0; p<transitionParticleCount; p++) {

						// Position
						playgroundCache.position[p] = Vector3.Lerp (transitionPosition[p], liveParticles?loadSnapshotData.position[p%loadParticleCount]:loadSnapshotData.targetPosition[p%loadParticleCount], t);
						if (!syncPositionsOnMainThread)
							particleCache[p].position = playgroundCache.position[p];

						// Color
						playgroundCache.color[p] = Color32.Lerp (transitionColor[p], loadSnapshotData.color[p%loadParticleCount], t);
						if (loadParticleCount<particleCount && p>=loadParticleCount)
							playgroundCache.color[p].a = (byte)Mathf.Lerp (transitionColor[p].a, 0f, t);
						particleCache[p].color = playgroundCache.color[p];

						// Size
						playgroundCache.size[p] = Mathf.Lerp (transitionSize[p], loadSnapshotData.size[p%loadParticleCount], t);
						particleCache[p].size = playgroundCache.size[p];

						// Rotation
						playgroundCache.rotation[p] = Mathf.Lerp (transitionRotation[p], loadSnapshotData.rotation[p%loadParticleCount], t);
						particleCache[p].rotation = playgroundCache.rotation[p];
					}
				});
				yield return null;
				if (firstFrameDone && currentParticleCount==particleCount) {
					if (syncPositionsOnMainThread)
						for (int p = 0; p<transitionParticleCount; p++) 
							particleCache[p].position = playgroundCache.position[p];
					shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
				}
				firstFrameDone = true;
			}

			if (loadParticleCount!=particleCount && abortTransition) {
				particleCache = new ParticleSystem.Particle[particleCount];
				shurikenParticleSystem.Emit(particleCount);
				shurikenParticleSystem.GetParticles(particleCache);
				shurikenParticleSystem.SetParticles(particleCache, particleCache.Length);
				System.Array.Resize(ref playgroundCache.position, particleCount);
				System.Array.Resize(ref playgroundCache.color, particleCount);
				System.Array.Resize(ref playgroundCache.size, particleCount);
				System.Array.Resize(ref playgroundCache.rotation, particleCount);
			}

			lastTimeUpdated = PlaygroundC.globalTime;
			cameFromNonCalculatedFrame = false;
			cameFromNonEmissionFrame = false;
			if (!abortTransition)
				inTransition = false;
		}

		float TransitionType (INDIVIDUALTRANSITIONTYPEC thisTransitionType, float t) {
			if (thisTransitionType==INDIVIDUALTRANSITIONTYPEC.Linear)
				return t;
			else if (thisTransitionType==INDIVIDUALTRANSITIONTYPEC.EaseIn)
				return Mathf.Lerp (0f, 1f, 1f-Mathf.Cos(t*Mathf.PI*.5f));
			else if (thisTransitionType==INDIVIDUALTRANSITIONTYPEC.EaseOut)
				return Mathf.Lerp (0f, 1f, Mathf.Sin(t*Mathf.PI*.5f));
			return t;
		}

		// Check all needed references
		void CheckReferences () {
			if (PlaygroundC.reference==null)
				PlaygroundC.reference = FindObjectOfType<PlaygroundC>();
			if (PlaygroundC.reference==null) {
				PlaygroundC.reference = PlaygroundC.ResourceInstantiate("Playground Manager").GetComponent<PlaygroundC>();
			}
			if (playgroundCache==null)
				playgroundCache = new PlaygroundCache();
			if (thisInstance==null)
				thisInstance = this;
			if (particleSystemGameObject==null) {
				particleSystemGameObject = gameObject;
				particleSystemTransform = transform;
				particleSystemRenderer = renderer;
				shurikenParticleSystem = particleSystemGameObject.GetComponent<ParticleSystem>();
				particleSystemRenderer2 = gameObject.particleSystem.renderer as ParticleSystemRenderer;
			}
		}

		// YieldedRefresh makes sure that Playground Manager and simulation time is ready before this particle system
		bool isYieldRefreshing = false;
		bool initialized = false;
		int initNearestNeighborSeq = 0;
		public IEnumerator YieldedRefresh () {
			if (isSnapshot) yield break;
			if (isYieldRefreshing) yield break;
			bool okToLoadFromStart = true;
			if (isSettingParticleCount || isSettingLifetime) okToLoadFromStart = false;
			while (!PlaygroundC.IsReady()) yield return null;
			while (isSettingParticleCount) yield return null;
			while (isSettingLifetime) yield return null;
			if (this.sorting==SORTINGC.NearestNeighbor || this.sorting==SORTINGC.NearestNeighborReversed) {
				initNearestNeighborSeq = 0;
				SetLifetime(thisInstance, SORTINGC.Burst, lifetime);
				isYieldRefreshing = false;
				initialized = true;
				yield break;
			}
			while (isSettingParticleCount || isSettingLifetime) yield return null;
			isYieldRefreshing = true;


			// Snapshot load
			#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				okToLoadFromStart = false;
			#endif
			if (okToLoadFromStart && loadFromStart && snapshots.Count>0) {
				SetLifetime(thisInstance, sorting, lifetime);
				Load(loadFrom);
				yield return null;
				isYieldRefreshing = false;
				initialized = true;
				yield break;
			}
			SetLifetime(thisInstance, sorting, lifetime);
			while (isSettingLifetime) yield return null;
			yield return null;
			transitionAvailable = true;
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;
			isYieldRefreshing = false;
			initialized = true;
		}

		public bool Initialized () {
			return initialized;
		}


		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// MonoBehaviours
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void Awake () {

			if (isSnapshot) return;

			// Make sure all references exists
			CheckReferences();

			if (thisInstance.enabled) {

				// Make sure that state data is initialized
				for (int x = 0; x<states.Count; x++)
					states[x].Initialize();
				
				// Initialize
				if (worldObject!=null && worldObject.transform!=null)
					worldObject.Initialize();
				if (skinnedWorldObject!=null && skinnedWorldObject.transform!=null)
					skinnedWorldObject.Initialize();
				if (projection!=null && projection.projectionTexture)
					projection.Initialize();
				if (manipulators.Count>0) {
					for (int i = 0; i<manipulators.Count; i++)
						manipulators[i].Update();
				}
			}

			//eventParticle = new PlaygroundEventParticle();

			if (PlaygroundC.reference!=null) {
				#if UNITY_EDITOR
				if (isSnapshot && !PlaygroundC.reference.showSnapshotsInHierarchy) {
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					return;
				}
				#endif
				if (isSnapshot) return;
				if (!PlaygroundC.reference.particleSystems.Contains(thisInstance))
					PlaygroundC.reference.particleSystems.Add(thisInstance);
				if (particleSystemTransform.parent==null && PlaygroundC.reference.autoGroup)
					particleSystemTransform.parent = PlaygroundC.referenceTransform;
			}

			// Reset event controlled by-list, this will be refreshed first calculation
			eventControlledBy = new List<PlaygroundParticlesC>();
		}

		void OnEnable () {
			if (isSnapshot || isLoading) return;
			thisInstance = this;
			initialized = false;
			isYieldRefreshing = false;
			isSettingParticleCount = false;
			isSettingLifetime = false;
			hasActiveParticles = true;
			threadHadNoActiveParticles = false;
			isDoneThread = true;
			calculate = true;
			lastTimeUpdated = PlaygroundC.globalTime;

			// Set new randoms
			RefreshSystemRandom();

			// Set 0 size to avoid one-frame flash
			shurikenParticleSystem.startSize = 0f;
			if (shurikenParticleSystem.isPaused || shurikenParticleSystem.isStopped)
				shurikenParticleSystem.Play();

			// Set initial values
			previousLifetimeValueMethod = lifetimeValueMethod;
			previousLifetime = lifetime;
			previousLifetimeMin = lifetimeMin;
			previousEmission = emit;
			previousLoop = loop;
			previousNearestNeighborOrigin = nearestNeighborOrigin;
			previousSorting = sorting;
			previousParticleCount = particleCount;
			loopExceeded = false;
			loopExceededOnParticle = -1;
			queueEmissionHalt = false;
			stPos = Vector3.zero;
			stRot = Quaternion.identity;
			stSca = new Vector3(1f, 1f, 1f);
			stDir = new Vector3();
			
			// Initiate all arrays by setting particle count
			if (particleCache==null) {
				SetParticleCount(thisInstance, particleCount);
			} else {
			
				// Clean up particle positions
				SetInitialTargetPosition(thisInstance, PlaygroundC.initialTargetPosition, true);

				// Refresh
				StartCoroutine(YieldedRefresh());
			}
		}

		public void Start () {
			if (gameObject.activeInHierarchy && gameObject.activeSelf)
				StartCoroutine (Boot());
		}

		public IEnumerator Boot () {
			if (particleSystemGameObject.activeInHierarchy && particleSystemGameObject.activeSelf && !isLoading && enabled) {

				while (PlaygroundC.reference==null) yield return null;

				// Update id if needed
				for (int i = 0; i<PlaygroundC.reference.particleSystems.Count; i++) {
					if (PlaygroundC.reference.particleSystems[i].particleSystemId==particleSystemId && PlaygroundC.reference.particleSystems[i]!=this) {
						particleSystemId = PlaygroundC.reference.particleSystems.Count;
						break;
					}
				}

				// Refresh
				StartCoroutine(YieldedRefresh());
			}
		}

		IEnumerator OnBecameInvisible () {
			if (isSnapshot) yield break;
			if (particleSystemGameObject.activeSelf) {
				if (forceVisibilityWhenOutOfFrustrum) {
					yield return null;
					shurikenParticleSystem.Play();
				}
				if (!pauseCalculationWhenInvisible)
					yield break;
				yield return new WaitForSeconds(1f);
				if (!particleSystemRenderer.isVisible && pauseCalculationWhenInvisible) {
					calculate = false;
					while (!particleSystemRenderer.isVisible && particleSystemGameObject.activeSelf) {
						yield return null;
					}
					if (pauseCalculationWhenInvisible)
						calculate = true;
				}
			}
		}

		void OnBecameVisible () {
			if (isSnapshot) return;
			if (pauseCalculationWhenInvisible) {
				calculate = true;
			}
		}

		void OnDestroy () {

			// Remove any events listed by other particle systems
			for (int i = 0; i<events.Count; i++) {
				if (events[i].target!=null)
					events[i].target.eventControlledBy.Remove (thisInstance);
			}

			// Remove this PlaygroundParticlesC object from Particle Systems list
			if (PlaygroundC.reference)
				PlaygroundC.reference.particleSystems.Remove(thisInstance);
		}

	#if UNITY_EDITOR
		// Select the particle system in Editor from MonoBehavior as we need one frame to initialize
		public void EditorYieldSelect () {StartCoroutine (YieldSelect ());}
		public IEnumerator YieldSelect () {
			if (isSnapshot || Application.isPlaying) yield break;
			yield return null;
			if (!UnityEditor.EditorApplication.isPlaying)
				UnityEditor.Selection.activeGameObject = particleSystemGameObject;
		}
	#endif
		
	}	

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MeshParticles - Extension class for PlaygroundParticlesC which creates mesh states. 
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public class MeshParticles : PlaygroundParticlesC {
		
		public static PlaygroundParticlesC CreateMeshParticles (Mesh[] meshes, Texture2D[] textures, Texture2D[] heightMap, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3[] offsets, Material material) {
			PlaygroundParticlesC meshParticles = PlaygroundParticlesC.CreateParticleObject(name,position,rotation,particleScale,material);
			meshParticles.states = new List<ParticleStateC>();
			
			int[] quantityList = new int[meshes.Length];
			int i = 0;
			for (; i<textures.Length; i++)
				quantityList[i] = meshes[i].vertexCount;
			meshParticles.particleCache = new ParticleSystem.Particle[quantityList[PlaygroundC.Largest(quantityList)]];
			meshParticles.shurikenParticleSystem.Emit(meshParticles.particleCache.Length);
			meshParticles.shurikenParticleSystem.GetParticles(meshParticles.particleCache);
			for (i = 0; i<meshes.Length; i++) {
				meshParticles.states.Add(new ParticleStateC());
				meshParticles.states[0].ConstructParticles(meshes[i],textures[i],particleScale,offsets[i], "State "+i,null);
			}
			
			meshParticles.UpdateSystem();
			PlaygroundC.particlesQuantity++;
			
			return meshParticles;
		}
		
		public static void Add (PlaygroundParticlesC meshParticles, Mesh mesh, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			meshParticles.states.Add(new ParticleStateC());
			meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,scale,offset,stateName,stateTransform);
		}
		
		public static void Add (PlaygroundParticlesC meshParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			meshParticles.states.Add(new ParticleStateC());
			meshParticles.states[meshParticles.states.Count-1].ConstructParticles(mesh,texture,scale,offset,stateName,stateTransform);
		}
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Cache
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Holds information for a particle system Snapshot.
	/// </summary>
	[Serializable]
	public class PlaygroundSave {
		[HideInInspector] public string name;										// The name of this PlaygroundSave
		[HideInInspector] public float time;										// The global time when this PlaygroundSave was created
		[HideInInspector] public float lifetime;									// The lifetime of the particle system
		[HideInInspector] public int particleCount;									// The particle count of the particle system
		[HideInInspector] public PlaygroundParticlesC settings;						// The cached settings of the particle system (Particles are stored in settings.snapshotData)
		[HideInInspector] public PlaygroundTransformC transform;					// The stored transform information
		[HideInInspector] public Material material;									// The stored material
		[HideInInspector] public int loadMode = 0;									// 0 = Settings+Particles, 1 = Settings only, 2 = Particles only
		[HideInInspector] public bool loadTransform = true;							// Should the stored transform information load?
		[HideInInspector] public bool loadMaterial = true;							// Should the stored material load?
		[HideInInspector] public bool setMaterialAfterTransition = true;			// Should the material load before or after transition?
		[HideInInspector] public float transitionMultiplier = 1f;					// The multiplier of transition time
		[HideInInspector] public INDIVIDUALTRANSITIONTYPEC transitionType;			// The transition type of this PlaygroundSave
		[HideInInspector] public bool unfolded = false;
		bool isLoading = false;

		/// <summary>
		/// Determines whether this PlaygroundSave is loading.
		/// </summary>
		/// <returns><c>true</c> if this PlaygroundSave is loading; otherwise, <c>false</c>.</returns>
		public bool IsLoading () {
			return isLoading;
		}

		/// <summary>
		/// Loads the data from this PlaygroundSave.
		/// </summary>
		/// <param name="loadTo">Load to.</param>
		public void Load (PlaygroundParticlesC loadTo) {
			if (loadMaterial && (setMaterialAfterTransition||!loadTo.loadTransition) && settings.particleSystemRenderer.sharedMaterial!=null) 
				loadTo.particleSystemRenderer.sharedMaterial = settings.particleSystemRenderer.sharedMaterial;
			isLoading = true;
			PlaygroundC.RunAsync(()=>{
				switch (loadMode) {
				case 0: settings.CopyTo(loadTo); loadTo.playgroundCache = settings.snapshotData.Clone(); break;
				case 1: settings.CopyTo(loadTo); break;
				case 2: loadTo.playgroundCache = settings.snapshotData.Clone(); break;
				default: settings.CopyTo(loadTo); loadTo.playgroundCache = settings.snapshotData.Clone(); break;
				}
				isLoading = false;
			});
			if (loadTransform)
				transform.GetFromTransform (loadTo.particleSystemTransform);

		}

		/// <summary>
		/// Saves the data into this PlaygroundSave.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public void Save (PlaygroundParticlesC playgroundParticles) {
			transform = new PlaygroundTransformC();
			transform.SetFromTransform (playgroundParticles.particleSystemTransform);
			settings.particleSystemRenderer.sharedMaterial = playgroundParticles.particleSystemRenderer.sharedMaterial;
			PlaygroundC.RunAsync(()=>{
				playgroundParticles.CopyTo(settings);
				settings.snapshotData = playgroundParticles.playgroundCache.Clone();
			});
		}

		/// <summary>
		/// Returns a copy of this PlaygroundSave.
		/// </summary>
		public PlaygroundSave Clone () {
			PlaygroundSave playgroundSave = new PlaygroundSave();
			settings.CopyTo(playgroundSave.settings);
			playgroundSave.name = name;
			playgroundSave.time = time;
			playgroundSave.lifetime = lifetime;
			playgroundSave.particleCount = particleCount;
			playgroundSave.loadMode = loadMode;
			playgroundSave.loadTransform = loadTransform;
			playgroundSave.loadMaterial = loadMaterial;
			playgroundSave.setMaterialAfterTransition = setMaterialAfterTransition;
			playgroundSave.material = material;
			playgroundSave.transitionMultiplier = transitionMultiplier;
			playgroundSave.transitionType = transitionType;
			playgroundSave.unfolded = unfolded;
			return playgroundSave;
		}
	}

	/// <summary>
	/// Holds information for a PlaygroundEventParticle. The Playground Event Particle contains detailed data upon an event and is sent towards any event listeners.
	/// </summary>
	[Serializable]
	public class PlaygroundEventParticle {
		/// <summary>
		/// The initial size of this particle.
		/// </summary>
		[HideInInspector] public float initialSize;									
		/// <summary>
		/// The lifetime size of this particle.
		/// </summary>
		[HideInInspector] public float size;										
		/// <summary>
		/// The rotation of this particle.
		/// </summary>
		[HideInInspector] public float rotation;									
		/// <summary>
		/// The lifetime of this particle.
		/// </summary>
		[HideInInspector] public float life;										
		/// <summary>
		/// The time of birth for this particle.
		/// </summary>
		[HideInInspector] public float birth;										
		/// <summary>
		/// The delayed time of birth when emission has changed.
		/// </summary>
		[HideInInspector] public float birthDelay;									
		/// <summary>
		/// The time of death for this particle.
		/// </summary>
		[HideInInspector] public float death;										
		/// <summary>
		/// The emission for this particle (controlled by emission rate).
		/// </summary>
		[HideInInspector] public bool emission;										
		/// <summary>
		/// Determines if this particle should rebirth.
		/// </summary>
		[HideInInspector] public bool rebirth;										
		/// <summary>
		/// The offset in birth-death (sorting).
		/// </summary>
		[HideInInspector] public float lifetimeOffset;								
		/// <summary>
		/// The velocity of this particle.
		/// </summary>
		[HideInInspector] public Vector3 velocity;									
		/// <summary>
		/// The initial velocity of this particle.
		/// </summary>
		[HideInInspector] public Vector3 initialVelocity;							
		/// <summary>
		/// The initial local velocity of this particle.
		/// </summary>
		[HideInInspector] public Vector3 initialLocalVelocity;						
		/// <summary>
		/// The position of this particle.
		/// </summary>
		[HideInInspector] public Vector3 position;									
		/// <summary>
		/// The source position at birth for this particle.
		/// </summary>
		[HideInInspector] public Vector3 targetPosition;							
		/// <summary>
		/// The source direction at birth for this particle.
		/// </summary>
		[HideInInspector] public Vector3 targetDirection;							
		/// <summary>
		/// The previous source position for this particle (used to calculate delta movement).
		/// </summary>
		[HideInInspector] public Vector3 previousTargetPosition; 					
		/// <summary>
		/// The previous calculated frame's particle position.
		/// </summary>
		[HideInInspector] public Vector3 previousParticlePosition;					
		/// <summary>
		/// The calculated particle position for collision.
		/// </summary>
		[HideInInspector] public Vector3 collisionParticlePosition;					
		/// <summary>
		/// The delta to compensate for moving particles in local space.
		/// </summary>
		[HideInInspector] public Vector3 localSpaceMovementCompensation;			
		/// <summary>
		/// The scattered position to apply on this particle birth.
		/// </summary>
		[HideInInspector] public Vector3 scatterPosition;							
		/// <summary>
		/// The initial rotation of this particle.
		/// </summary>
		[HideInInspector] public float initialRotation;								
		/// <summary>
		/// The rotation speed of this particle.
		/// </summary>
		[HideInInspector] public float rotationSpeed;								
		/// <summary>
		/// The current color of this particle.
		/// </summary>
		[HideInInspector] public Color32 color;										
		/// <summary>
		/// The color set from script of this particle.
		/// </summary>
		[HideInInspector] public Color32 scriptedColor;								
		/// <summary>
		/// The set source color.
		/// </summary>
		[HideInInspector] public Color32 initialColor;								
		/// <summary>
		/// The color gradient for this particle if Color Source is set to LifetimeColors.
		/// </summary>
		[HideInInspector] public int lifetimeColorId;								
		/// <summary>
		/// The particle does not respond to forces during its lifetime.
		/// </summary>
		[HideInInspector] public bool noForce;
		/// <summary>
		/// The particle is non birthed.
		/// </summary>
		[HideInInspector] public bool isNonBirthed;
		/// <summary>
		/// The particle is in its first loop.
		/// </summary>
		[HideInInspector] public bool isFirstLoop;
		/// <summary>
		/// The id of this particle.
		/// </summary>
		[HideInInspector] public int particleId;									
		/// <summary>
		/// The id of the particle system this particle belongs to (list position in Playground Manager).
		/// </summary>
		[HideInInspector] public int particleSystemId;								
		/// <summary>
		/// The id of the last manipulator affecting this particle.
		/// </summary>
		[HideInInspector] public int manipulatorId;									

		/// <summary>
		/// The interaction with property manipulators of this particle.
		/// </summary>
		[HideInInspector] public bool changedByProperty;							
		/// <summary>
		/// The interaction with property manipulators that change color of this particle.
		/// </summary>
		[HideInInspector] public bool changedByPropertyColor;						
		/// <summary>
		/// The interaction with property manipulators that change color over time of this particle.
		/// </summary>
		[HideInInspector] public bool changedByPropertyColorLerp; 					
		/// <summary>
		/// The interaction with property manipulators that change color and wants to keep alpha.
		/// </summary>
		[HideInInspector] public bool changedByPropertyColorKeepAlpha;				
		/// <summary>
		/// The interaction with property manipulators that change size of this particle.
		/// </summary>
		[HideInInspector] public bool changedByPropertySize;						
		/// <summary>
		/// The interaction with property manipulators that change target of this particle.
		/// </summary>
		[HideInInspector] public bool changedByPropertyTarget;						
		/// <summary>
		/// The interaction with death manipulators that forces a particle to a sooner end.
		/// </summary>
		[HideInInspector] public bool changedByPropertyDeath;						
		/// <summary>
		/// The property target pointer for this particle.
		/// </summary>
		[HideInInspector] public int propertyTarget;								
		/// <summary>
		/// The property target id for this particle (pairing a particle's target to a manipulator).
		/// </summary>
		[HideInInspector] public int propertyId;									
		/// <summary>
		/// The property color id for this particles (pairing a particle's color to a manipulator).
		/// </summary>
		[HideInInspector] public int propertyColorId;								
		/// <summary>
		/// The manipulator id to exclude to not affect this particle.
		/// </summary>
		[HideInInspector] public int excludeFromManipulatorId;						

		/// <summary>
		/// Is this particle masked?
		/// </summary>
		[HideInInspector] public bool isMasked;									
		/// <summary>
		///  The alpha of this masked particle.
		/// </summary>
		[HideInInspector] public float maskAlpha;								

		/// <summary>
		/// The collision transform of this collided particle.
		/// </summary>
		[HideInInspector] public Transform collisionTransform;
		/// <summary>
		/// The collision collider of this collided particle (3d).
		/// </summary>
		[HideInInspector] public Collider collisionCollider;
		/// <summary>
		/// The collision collider of this collided particle (2d).
		/// </summary>
		[HideInInspector] public Collider2D collisionCollider2D;

		/// <summary>
		/// Copies this PlaygroundEventParticle.
		/// </summary>
		public PlaygroundEventParticle Clone () {
			PlaygroundEventParticle playgroundEventParticle = new PlaygroundEventParticle();
			playgroundEventParticle.initialSize = initialSize;
			playgroundEventParticle.size = size;
			playgroundEventParticle.life = life;
			playgroundEventParticle.rotation = rotation;
			playgroundEventParticle.birth = birth;
			playgroundEventParticle.birthDelay = birthDelay;
			playgroundEventParticle.death = death;
			playgroundEventParticle.emission = emission;
			playgroundEventParticle.rebirth = rebirth;
			playgroundEventParticle.lifetimeOffset = lifetimeOffset;
			playgroundEventParticle.velocity = velocity;
			playgroundEventParticle.initialVelocity = initialVelocity;
			playgroundEventParticle.initialLocalVelocity = initialLocalVelocity;
			playgroundEventParticle.position = position;
			playgroundEventParticle.targetPosition = targetPosition;
			playgroundEventParticle.targetDirection = targetDirection;
			playgroundEventParticle.previousTargetPosition = previousTargetPosition;
			playgroundEventParticle.previousParticlePosition = previousParticlePosition;
			playgroundEventParticle.collisionParticlePosition = collisionParticlePosition;
			playgroundEventParticle.localSpaceMovementCompensation = localSpaceMovementCompensation;
			playgroundEventParticle.scatterPosition = scatterPosition;
			playgroundEventParticle.initialRotation = initialRotation;
			playgroundEventParticle.rotationSpeed = rotationSpeed;
			playgroundEventParticle.color = color;
			playgroundEventParticle.scriptedColor = scriptedColor;
			playgroundEventParticle.initialColor = initialColor;
			playgroundEventParticle.lifetimeColorId = lifetimeColorId;
			playgroundEventParticle.noForce = noForce;
			playgroundEventParticle.changedByProperty = changedByProperty;
			playgroundEventParticle.changedByPropertyColor = changedByPropertyColor;
			playgroundEventParticle.changedByPropertyColorLerp = changedByPropertyColorLerp;
			playgroundEventParticle.changedByPropertyColorKeepAlpha = changedByPropertyColorKeepAlpha;
			playgroundEventParticle.changedByPropertySize = changedByPropertySize;
			playgroundEventParticle.changedByPropertyTarget = changedByPropertyTarget;
			playgroundEventParticle.changedByPropertyDeath = changedByPropertyDeath;
			playgroundEventParticle.propertyTarget = propertyTarget;
			playgroundEventParticle.propertyId = propertyId;
			playgroundEventParticle.excludeFromManipulatorId = excludeFromManipulatorId;
			playgroundEventParticle.propertyColorId = propertyColorId;
			playgroundEventParticle.particleId = particleId;
			playgroundEventParticle.particleSystemId = particleSystemId;
			playgroundEventParticle.manipulatorId = manipulatorId;
			playgroundEventParticle.isMasked = isMasked;
			playgroundEventParticle.maskAlpha = maskAlpha;

			playgroundEventParticle.collisionTransform = collisionTransform;
			playgroundEventParticle.collisionCollider = collisionCollider;
			playgroundEventParticle.collisionCollider2D = collisionCollider2D;
			
			return playgroundEventParticle;
		}
	}

	/// <summary>
	/// The Playground Cache contains all data for particles in built-in arrays.
	/// </summary>
	[Serializable]
	public class PlaygroundCache {
		/// <summary>
		/// The initial size of each particle.
		/// </summary>
		[HideInInspector] public float[] initialSize;								
		/// <summary>
		/// The lifetime size of each particle.
		/// </summary>
		[HideInInspector] public float[] size;										
		/// <summary>
		/// The rotation of each particle.
		/// </summary>
		[HideInInspector] public float[] rotation;									
		/// <summary>
		/// The current lifetime of each particle.
		/// </summary>
		[HideInInspector] public float[] life;										
		/// <summary>
		/// The lifetime subtraction of each particle (applied when using random between two values).
		/// </summary>
		[HideInInspector] public float[] lifetimeSubtraction;						
		/// <summary>
		/// The time of birth for each particle.
		/// </summary>
		[HideInInspector] public float[] birth;										
		/// <summary>
		/// The delayed time of birth when emission has changed.
		/// </summary>
		[HideInInspector] public float[] birthDelay;								
		/// <summary>
		/// The time of death for each particle.
		/// </summary>
		[HideInInspector] public float[] death;										
		/// <summary>
		/// The emission for each particle (controlled by emission rate).
		/// </summary>
		[HideInInspector] public bool[] emission;									
		/// <summary>
		/// The rebirth for each particle.
		/// </summary>
		[HideInInspector] public bool[] rebirth;									
		/// <summary>
		/// The offset in birth-death (sorting).
		/// </summary>
		[HideInInspector] public float[] lifetimeOffset;							
		/// <summary>
		/// The velocity of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Vector3[] velocity;								
		/// <summary>
		/// The initial velocity of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Vector3[] initialVelocity;							
		/// <summary>
		/// The initial local velocity of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Vector3[] initialLocalVelocity;					
		/// <summary>
		/// The position of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Vector3[] position;								
		/// <summary>
		/// The source position for each particle.
		/// </summary>
		[HideInInspector] public Vector3[] targetPosition;							
		/// <summary>
		/// The source direction for each particle.
		/// </summary>
		[HideInInspector] public Vector3[] targetDirection;							
		/// <summary>
		/// The previous source position for each particle (used to calculate delta movement).
		/// </summary>
		[HideInInspector] public Vector3[] previousTargetPosition; 					
		/// <summary>
		/// The previous calculated frame's particle position.
		/// </summary>
		[HideInInspector] public Vector3[] previousParticlePosition;				
		/// <summary>
		/// The calculated particle position for collision (depending on collision stepper).
		/// </summary>
		[HideInInspector] public Vector3[] collisionParticlePosition;				
		/// <summary>
		/// The delta to compensate for moving particles in local space.
		/// </summary>
		[HideInInspector] public Vector3[] localSpaceMovementCompensation;			
		/// <summary>
		/// The scattered position to apply on each particle birth in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Vector3[] scatterPosition;							
		/// <summary>
		/// The initial rotation of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public float[] initialRotation;							
		/// <summary>
		/// The rotation speed of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public float[] rotationSpeed;								
		/// <summary>
		/// The color of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Color32[] color;									
		/// <summary>
		/// The color set from script of each particle in this PlaygroundParticles.
		/// </summary>
		[HideInInspector] public Color32[] scriptedColor;							
		/// <summary>
		/// The set source color.
		/// </summary>
		[HideInInspector] public Color32[] initialColor;							
		/// <summary>
		/// The color gradient for each particle if Color Source is set to LifetimeColors.
		/// </summary>
		[HideInInspector] public int[] lifetimeColorId;								
		/// <summary>
		/// The particle will no longer respond to any forces during its lifetime.
		/// </summary>
		[HideInInspector] public bool[] noForce;
		/// <summary>
		/// The particle is not birthed yet.
		/// </summary>
		[HideInInspector] public bool[] isNonBirthed;
		/// <summary>
		/// The particle is in its first loop.
		/// </summary>
		[HideInInspector] public bool[] isFirstLoop;
		/// <summary>
		/// The particle is set to simulate.
		/// </summary>
		[HideInInspector] public bool[] simulate;

		// Manipulator specific arrays:
		/// <summary>
		/// The interaction with property manipulators of each particle.
		/// </summary>
		[HideInInspector] public bool[] changedByProperty;							
		/// <summary>
		/// The interaction with property manipulators that change color of each particle.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertyColor;						
		/// <summary>
		/// The interaction with property manipulators that change color over time of each particle.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertyColorLerp; 				
		/// <summary>
		/// The interaction with property manipulators that change color and wants to keep alpha.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertyColorKeepAlpha;			
		/// <summary>
		/// The interaction with property manipulators that change size of each particle.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertySize;						
		/// <summary>
		/// The interaction with property manipulators that change target of each particle.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertyTarget;					
		/// <summary>
		/// The interaction with death manipulators that forces a particle to a sooner end.
		/// </summary>
		[HideInInspector] public bool[] changedByPropertyDeath;						
		/// <summary>
		/// The property target pointer for each particle.
		/// </summary>
		[HideInInspector] public int[] propertyTarget;								
		/// <summary>
		/// The property target id for each particle (pairing a particle's target to a manipulator).
		/// </summary>
		[HideInInspector] public int[] propertyId;									
		/// <summary>
		/// The property color id for each particles (pairing a particle's color to a manipulator.
		/// </summary>
		[HideInInspector] public int[] propertyColorId;								
		/// <summary>
		/// The id of the last manipulator affecting this particle.
		/// </summary>
		[HideInInspector] public int[] manipulatorId;								
		/// <summary>
		/// The id of manipulator to not affect this particle.
		/// </summary>
		[HideInInspector] public int[] excludeFromManipulatorId;					

		/// <summary>
		/// Is this particle masked?
		/// </summary>
		[HideInInspector] public bool[] isMasked;									
		/// <summary>
		/// The alpha of this masked particle.
		/// </summary>
		[HideInInspector] public float[] maskAlpha;									
		
		/// <summary>
		/// Copies this PlaygroundCache.
		/// </summary>
		public PlaygroundCache Clone () {
			PlaygroundCache playgroundCache = new PlaygroundCache();
			playgroundCache.initialSize = initialSize.Clone() as float[];
			playgroundCache.size = size.Clone () as float[];
			playgroundCache.life = life.Clone() as float[];
			playgroundCache.lifetimeSubtraction = lifetimeSubtraction.Clone() as float[];
			playgroundCache.rotation = rotation.Clone() as float[];
			playgroundCache.birth = birth.Clone() as float[];
			playgroundCache.birthDelay = birthDelay.Clone() as float[];
			playgroundCache.death = death.Clone() as float[];
			playgroundCache.emission = emission.Clone() as bool[];
			playgroundCache.rebirth = rebirth.Clone() as bool[];
			playgroundCache.lifetimeOffset = lifetimeOffset.Clone() as float[];
			playgroundCache.velocity = velocity.Clone() as Vector3[];
			playgroundCache.initialVelocity = initialVelocity.Clone() as Vector3[];
			playgroundCache.initialLocalVelocity = initialLocalVelocity.Clone() as Vector3[];
			playgroundCache.position = position.Clone () as Vector3[];
			playgroundCache.targetPosition = targetPosition.Clone() as Vector3[];
			playgroundCache.targetDirection = targetDirection.Clone() as Vector3[];
			playgroundCache.previousTargetPosition = previousTargetPosition.Clone() as Vector3[];
			playgroundCache.previousParticlePosition = previousParticlePosition.Clone() as Vector3[];
			playgroundCache.collisionParticlePosition = collisionParticlePosition.Clone () as Vector3[];
			playgroundCache.localSpaceMovementCompensation = localSpaceMovementCompensation.Clone() as Vector3[];
			playgroundCache.scatterPosition = scatterPosition.Clone() as Vector3[];
			playgroundCache.initialRotation = initialRotation.Clone() as float[];
			playgroundCache.rotationSpeed = rotationSpeed.Clone() as float[];
			playgroundCache.color = color.Clone() as Color32[];
			playgroundCache.scriptedColor = scriptedColor.Clone() as Color32[];
			playgroundCache.initialColor = initialColor.Clone () as Color32[];
			playgroundCache.lifetimeColorId = lifetimeColorId.Clone () as int[];
			playgroundCache.noForce = noForce.Clone() as bool[];
			playgroundCache.changedByProperty = changedByProperty.Clone() as bool[];
			playgroundCache.changedByPropertyColor = changedByPropertyColor.Clone() as bool[];
			playgroundCache.changedByPropertyColorLerp = changedByPropertyColorLerp.Clone() as bool[];
			playgroundCache.changedByPropertyColorKeepAlpha = changedByPropertyColorKeepAlpha.Clone () as bool[];
			playgroundCache.changedByPropertySize = changedByPropertySize.Clone() as bool[];
			playgroundCache.changedByPropertyTarget = changedByPropertyTarget.Clone() as bool[];
			playgroundCache.changedByPropertyDeath = changedByPropertyDeath.Clone() as bool[];
			playgroundCache.propertyTarget = propertyTarget.Clone() as int[];
			playgroundCache.propertyId = propertyId.Clone() as int[];
			playgroundCache.excludeFromManipulatorId = excludeFromManipulatorId.Clone() as int[];
			playgroundCache.propertyColorId = propertyColorId.Clone() as int[];
			playgroundCache.manipulatorId = manipulatorId.Clone() as int[];
			playgroundCache.isMasked = isMasked.Clone() as bool[];
			playgroundCache.maskAlpha = maskAlpha.Clone() as float[];
			playgroundCache.isNonBirthed = isNonBirthed.Clone() as bool[];
			playgroundCache.isFirstLoop = isFirstLoop.Clone() as bool[];
			playgroundCache.simulate = simulate.Clone() as bool[];
			return playgroundCache;
		}
	}

	/// <summary>
	/// The Simplex Noise algorithm applied in Turbulence.
	/// </summary>
	public class SimplexNoise {

		// Simplex noise based on http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf for public domain by courtesy of Stefan Gustavson.
		private static int[][] grad3 = new int[][] {
			new int[] {1,1,0}, new int[] {-1,1,0}, new int[] {1,-1,0}, new int[] {-1,-1,0},
			new int[] {1,0,1}, new int[] {-1,0,1}, new int[] {1,0,-1}, new int[] {-1,0,-1},
			new int[] {0,1,1}, new int[] {0,-1,1}, new int[] {0,1,-1}, new int[] {0,-1,-1}};
		private static int[][] grad4 = new int[][] {
			new int[] {0,1,1,1},  new int[] {0,1,1,-1},  new int[] {0,1,-1,1},  new int[] {0,1,-1,-1},
			new int[] {0,-1,1,1}, new int[] {0,-1,1,-1}, new int[] {0,-1,-1,1}, new int[] {0,-1,-1,-1},
			new int[] {1,0,1,1},  new int[] {1,0,1,-1},  new int[] {1,0,-1,1},  new int[] {1,0,-1,-1},
			new int[] {-1,0,1,1}, new int[] {-1,0,1,-1}, new int[] {-1,0,-1,1}, new int[] {-1,0,-1,-1},
			new int[] {1,1,0,1},  new int[] {1,1,0,-1},  new int[] {1,-1,0,1},  new int[] {1,-1,0,-1},
			new int[] {-1,1,0,1}, new int[] {-1,1,0,-1}, new int[] {-1,-1,0,1}, new int[] {-1,-1,0,-1},
			new int[] {1,1,1,0},  new int[] {1,1,-1,0},  new int[] {1,-1,1,0},  new int[] {1,-1,-1,0},
			new int[] {-1,1,1,0}, new int[] {-1,1,-1,0}, new int[] {-1,-1,1,0}, new int[] {-1,-1,-1,0}};
		private static int[] p = {151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
			129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
			49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180};
		// To remove the need for index wrapping, double the permutation table length
		private static int[] perm = new int[512];
		static SimplexNoise() { for(int i=0; i<512; i++) perm[i]=p[i & 255]; } // moved to constructor
		// A lookup table to traverse the simplex around a given point in 4D.
		// Details can be found where this table is used, in the 4D noise method.
		private static int[][] simplex = new int[][] {
			new int[] {0,1,2,3}, new int[] {0,1,3,2}, new int[] {0,0,0,0}, new int[] {0,2,3,1}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {1,2,3,0},
			new int[] {0,2,1,3}, new int[] {0,0,0,0}, new int[] {0,3,1,2}, new int[] {0,3,2,1}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {1,3,2,0},
			new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0},
			new int[] {1,2,0,3}, new int[] {0,0,0,0}, new int[] {1,3,0,2}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {2,3,0,1}, new int[] {2,3,1,0},
			new int[] {1,0,2,3}, new int[] {1,0,3,2}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {2,0,3,1}, new int[] {0,0,0,0}, new int[] {2,1,3,0},
			new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0},
			new int[] {2,0,1,3}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {3,0,1,2}, new int[] {3,0,2,1}, new int[] {0,0,0,0}, new int[] {3,1,2,0},
			new int[] {2,1,0,3}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {0,0,0,0}, new int[] {3,1,0,2}, new int[] {0,0,0,0}, new int[] {3,2,0,1}, new int[] {3,2,1,0}};
		// This method is a *lot* faster than using (int)Mathf.floor(x)
		private static int fastfloor(double x) {
			return x>0 ? (int)x : (int)x-1;
		}
		private static double dot(int[] g, double x, double y) {
			return g[0]*x + g[1]*y;
		}
		private static double dot(int[] g, double x, double y, double z) {
			return g[0]*x + g[1]*y + g[2]*z;
		}
		private static double dot(int[] g, double x, double y, double z, double w) {
			return g[0]*x + g[1]*y + g[2]*z + g[3]*w; 
		}

		// 3D simplex noise
		public double noise(double xin, double yin, double zin) {
			double n0, n1, n2, n3; // Noise contributions from the four corners
			// Skew the input space to determine which simplex cell we're in
			double F3 = 1.0/3.0;
			double s = (xin+yin+zin)*F3; // Very nice and simple skew factor for 3D
			int i = fastfloor(xin+s);
			int j = fastfloor(yin+s);
			int k = fastfloor(zin+s);
			double G3 = 1.0/6.0; // Very nice and simple unskew factor, too
			double t = (i+j+k)*G3; 
			double X0 = i-t; // Unskew the cell origin back to (x,y,z) space
			double Y0 = j-t;
			double Z0 = k-t;
			double x0 = xin-X0; // The x,y,z distances from the cell origin
			double y0 = yin-Y0;
			double z0 = zin-Z0;
			// For the 3D case, the simplex shape is a slightly irregular tetrahedron.
			// Determine which simplex we are in.
			int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
			int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords
			if(x0>=y0) {
				if(y0>=z0)
				{ i1=1; j1=0; k1=0; i2=1; j2=1; k2=0; } // X Y Z order
				else if(x0>=z0) { i1=1; j1=0; k1=0; i2=1; j2=0; k2=1; } // X Z Y order
				else { i1=0; j1=0; k1=1; i2=1; j2=0; k2=1; } // Z X Y order
			}
			else { // x0<y0
				if(y0<z0) { i1=0; j1=0; k1=1; i2=0; j2=1; k2=1; } // Z Y X order
				else if(x0<z0) { i1=0; j1=1; k1=0; i2=0; j2=1; k2=1; } // Y Z X order
				else { i1=0; j1=1; k1=0; i2=1; j2=1; k2=0; } // Y X Z order
			}
			// A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
			// a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
			// a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
			// c = 1/6.
			double x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
			double y1 = y0 - j1 + G3;
			double z1 = z0 - k1 + G3;
			double x2 = x0 - i2 + 2.0*G3; // Offsets for third corner in (x,y,z) coords
			double y2 = y0 - j2 + 2.0*G3;
			double z2 = z0 - k2 + 2.0*G3;
			double x3 = x0 - 1.0 + 3.0*G3; // Offsets for last corner in (x,y,z) coords
			double y3 = y0 - 1.0 + 3.0*G3;
			double z3 = z0 - 1.0 + 3.0*G3;
			// Work out the hashed gradient indices of the four simplex corners
			int ii = i & 255;
			int jj = j & 255;
			int kk = k & 255;
			int gi0 = perm[ii+perm[jj+perm[kk]]] % 12;
			int gi1 = perm[ii+i1+perm[jj+j1+perm[kk+k1]]] % 12;
			int gi2 = perm[ii+i2+perm[jj+j2+perm[kk+k2]]] % 12;
			int gi3 = perm[ii+1+perm[jj+1+perm[kk+1]]] % 12;
			// Calculate the contribution from the four corners
			double t0 = 0.5 - x0*x0 - y0*y0 - z0*z0;
			if(t0<0) n0 = 0.0;
			else {
				t0 *= t0;
				n0 = t0 * t0 * dot(grad3[gi0], x0, y0, z0);
			}
			double t1 = 0.6 - x1*x1 - y1*y1 - z1*z1;
			if(t1<0) n1 = 0.0;
			else {
				t1 *= t1;
				n1 = t1 * t1 * dot(grad3[gi1], x1, y1, z1);
			}
			double t2 = 0.6 - x2*x2 - y2*y2 - z2*z2;
			if(t2<0) n2 = 0.0;
			else {
				t2 *= t2;
				n2 = t2 * t2 * dot(grad3[gi2], x2, y2, z2);
			}
			double t3 = 0.6 - x3*x3 - y3*y3 - z3*z3;
			if(t3<0) n3 = 0.0;
			else {
				t3 *= t3;
				n3 = t3 * t3 * dot(grad3[gi3], x3, y3, z3);
			}
			// Add contributions from each corner to get the final noise value.
			// The result is scaled to stay just inside [-1,1]
			return 32.0*(n0 + n1 + n2 + n3);
		}  

		// 4D simplex noise
		public double noise(double x, double y, double z, double w) {
			
			// The skewing and unskewing factors are hairy again for the 4D case
			double F4 = (Mathf.Sqrt(5.0f)-1.0)/4.0;
			double G4 = (5.0-Mathf.Sqrt(5.0f))/20.0;
			double n0, n1, n2, n3, n4; // Noise contributions from the five corners
			// Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
			double s = (x + y + z + w) * F4; // Factor for 4D skewing
			int i = fastfloor(x + s);
			int j = fastfloor(y + s);
			int k = fastfloor(z + s);
			int l = fastfloor(w + s);
			double t = (i + j + k + l) * G4; // Factor for 4D unskewing
			double X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
			double Y0 = j - t;
			double Z0 = k - t;
			double W0 = l - t;
			double x0 = x - X0;  // The x,y,z,w distances from the cell origin
			double y0 = y - Y0;
			double z0 = z - Z0;
			double w0 = w - W0;
			// For the 4D case, the simplex is a 4D shape I won't even try to describe.
			// To find out which of the 24 possible simplices we're in, we need to
			// determine the magnitude ordering of x0, y0, z0 and w0.
			// The method below is a good way of finding the ordering of x,y,z,w and
			// then find the correct traversal order for the simplex we’re in.
			// First, six pair-wise comparisons are performed between each possible pair
			// of the four coordinates, and the results are used to add up binary bits
			// for an integer index.
			int c1 = (x0 > y0) ? 32 : 0;
			int c2 = (x0 > z0) ? 16 : 0;
			int c3 = (y0 > z0) ? 8 : 0;
			int c4 = (x0 > w0) ? 4 : 0;
			int c5 = (y0 > w0) ? 2 : 0;
			int c6 = (z0 > w0) ? 1 : 0;
			int c = c1 + c2 + c3 + c4 + c5 + c6;
			int i1, j1, k1, l1; // The integer offsets for the second simplex corner
			int i2, j2, k2, l2; // The integer offsets for the third simplex corner
			int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner
			// simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
			// Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
			// impossible. Only the 24 indices which have non-zero entries make any sense.
			// We use a thresholding to set the coordinates in turn from the largest magnitude.
			// The number 3 in the "simplex" array is at the position of the largest coordinate.
			i1 = simplex[c][0]>=3 ? 1 : 0;
			j1 = simplex[c][1]>=3 ? 1 : 0;
			k1 = simplex[c][2]>=3 ? 1 : 0;
			l1 = simplex[c][3]>=3 ? 1 : 0;
			// The number 2 in the "simplex" array is at the second largest coordinate.
			i2 = simplex[c][0]>=2 ? 1 : 0;
			j2 = simplex[c][1]>=2 ? 1 : 0;    k2 = simplex[c][2]>=2 ? 1 : 0;
			l2 = simplex[c][3]>=2 ? 1 : 0;
			// The number 1 in the "simplex" array is at the second smallest coordinate.
			i3 = simplex[c][0]>=1 ? 1 : 0;
			j3 = simplex[c][1]>=1 ? 1 : 0;
			k3 = simplex[c][2]>=1 ? 1 : 0;
			l3 = simplex[c][3]>=1 ? 1 : 0;
			// The fifth corner has all coordinate offsets = 1, so no need to look that up.
			double x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
			double y1 = y0 - j1 + G4;
			double z1 = z0 - k1 + G4;
			double w1 = w0 - l1 + G4;
			double x2 = x0 - i2 + 2.0*G4; // Offsets for third corner in (x,y,z,w) coords
			double y2 = y0 - j2 + 2.0*G4;
			double z2 = z0 - k2 + 2.0*G4;
			double w2 = w0 - l2 + 2.0*G4;
			double x3 = x0 - i3 + 3.0*G4; // Offsets for fourth corner in (x,y,z,w) coords
			double y3 = y0 - j3 + 3.0*G4;
			double z3 = z0 - k3 + 3.0*G4;
			double w3 = w0 - l3 + 3.0*G4;
			double x4 = x0 - 1.0 + 4.0*G4; // Offsets for last corner in (x,y,z,w) coords
			double y4 = y0 - 1.0 + 4.0*G4;
			double z4 = z0 - 1.0 + 4.0*G4;
			double w4 = w0 - 1.0 + 4.0*G4;
			// Work out the hashed gradient indices of the five simplex corners
			int ii = i & 255;
			int jj = j & 255;
			int kk = k & 255;
			int ll = l & 255;
			int gi0 = perm[ii+perm[jj+perm[kk+perm[ll]]]] % 32;
			int gi1 = perm[ii+i1+perm[jj+j1+perm[kk+k1+perm[ll+l1]]]] % 32;
			int gi2 = perm[ii+i2+perm[jj+j2+perm[kk+k2+perm[ll+l2]]]] % 32;
			int gi3 = perm[ii+i3+perm[jj+j3+perm[kk+k3+perm[ll+l3]]]] % 32;
			int gi4 = perm[ii+1+perm[jj+1+perm[kk+1+perm[ll+1]]]] % 32;
			// Calculate the contribution from the five corners
			double t0 = 0.5 - x0*x0 - y0*y0 - z0*z0 - w0*w0;
			if(t0<0) n0 = 0.0;
			else {
				t0 *= t0;
				n0 = t0 * t0 * dot(grad4[gi0], x0, y0, z0, w0);
			}
			double t1 = 0.6 - x1*x1 - y1*y1 - z1*z1 - w1*w1;
			if(t1<0) n1 = 0.0;
			else {
				t1 *= t1;
				n1 = t1 * t1 * dot(grad4[gi1], x1, y1, z1, w1);
			}
			double t2 = 0.6 - x2*x2 - y2*y2 - z2*z2 - w2*w2;
			if(t2<0) n2 = 0.0;
			else {
				t2 *= t2;
				n2 = t2 * t2 * dot(grad4[gi2], x2, y2, z2, w2);
			}   double t3 = 0.6 - x3*x3 - y3*y3 - z3*z3 - w3*w3;
			if(t3<0) n3 = 0.0;
			else {
				t3 *= t3;
				n3 = t3 * t3 * dot(grad4[gi3], x3, y3, z3, w3);
			}
			double t4 = 0.6 - x4*x4 - y4*y4 - z4*z4 - w4*w4;
			if(t4<0) n4 = 0.0;
			else {
				t4 *= t4;
				n4 = t4 * t4 * dot(grad4[gi4], x4, y4, z4, w4);
			}
			// Sum up and scale the result to cover the range [-1,1]
			return 27.0 * (n0 + n1 + n2 + n3 + n4);
		}

	}
}
