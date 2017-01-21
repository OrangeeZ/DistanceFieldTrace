using UnityEngine;

namespace WarFog {

	public class WarFogRenderer : MonoBehaviour {

		public static WarFogRenderer Instance { get; private set; }

		[SerializeField]
		private Shader _raytraceShader;

		[SerializeField]
		private Shader _blendShader;

		private Material _raytraceMaterial;
		private Material _blendMaterial;
		private Texture2D _warFogTexture;

		private float _brightness = 1f;

		private void Awake() {

			Instance = this;

			_raytraceMaterial = new Material( _raytraceShader );
		}

		// Use this for initialization
		private void Start() {

			SetBrightness( 1f );

			GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		}

		private void OnRenderImage( RenderTexture src, RenderTexture dest ) {

			//var blurredWarFog = _blurOptimized.BlurTexture( _warFogTexture );

			//_raytraceMaterial.SetTexture( "_WarFogTexture", blurredWarFog );

			var halfResBufer = RenderTexture.GetTemporary( Screen.width / 2, Screen.height / 2, 0 );
			Graphics.Blit( src, halfResBufer, _raytraceMaterial );

			Graphics.Blit( halfResBufer, dest );
			RenderTexture.ReleaseTemporary( halfResBufer );
			//Graphics.Blit( blurredWarFog, dest, _raytraceMaterial );

			//RenderTexture.ReleaseTemporary( blurredWarFog );
		}

//		public void SetTexture( WarFogSpaceMap spaceMap, Texture2D warFogTexture ) {
//
//			_warFogTexture = warFogTexture;
//
//			_raytraceMaterial.SetTexture( "_WarFogTexture", _warFogTexture );
//
//			_raytraceMaterial.SetFloat( "_WarFogBrightness", _brightness );
//
//			var spaceMapBounds = spaceMap.GetBounds();
//			_raytraceMaterial.SetMatrix( "_World2Texture", Matrix4x4.TRS( Vector3.zero, Quaternion.identity, new Vector3( 1f / spaceMapBounds.size.x, 0, 1f / spaceMapBounds.size.z ) ) );
//
//			var inverseViewProjectionMatrix = ( Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix ).inverse;
//			_raytraceMaterial.SetMatrix( "_ViewProjectInverse", inverseViewProjectionMatrix );
//
//			_raytraceMaterial.SetMatrix( "_Camera2World", Camera.main.cameraToWorldMatrix );
//		}

		public void SetTexture( DistanceField distanceField, Texture2D warFogTexture ) {

			_warFogTexture = warFogTexture;

			_raytraceMaterial.SetTexture( "_WarFogTexture", _warFogTexture );
			
			_raytraceMaterial.SetFloat( "_WarFogBrightness", _brightness );
			_raytraceMaterial.SetFloat( "_MaxFieldDistance", distanceField.GetMaxFieldDistance());

			var spaceMapBounds = distanceField.GetBounds();
			_raytraceMaterial.SetMatrix( "_World2Texture", Matrix4x4.TRS( Vector3.zero, Quaternion.identity, new Vector3( 1f / spaceMapBounds.size.x, 0, 1f / spaceMapBounds.size.z ) ) );

			var inverseViewProjectionMatrix = GL.GetGPUProjectionMatrix( Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix, renderIntoTexture: false ).inverse;
			_raytraceMaterial.SetMatrix( "_ViewProjectInverse", inverseViewProjectionMatrix );

			_raytraceMaterial.SetMatrix( "_Camera2World", Camera.main.cameraToWorldMatrix );
		}

		public void SetTracerPosition( Vector3 position ) {

			_raytraceMaterial.SetVector( "_WorldTracerPosition", position );
		}

		public void SetBrightness( float value ) {

			_brightness = value;
		}

	}

}