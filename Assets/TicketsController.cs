﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TicketsController : MonoBehaviour {
	
	public GameObject player;
	public GameObject score;
	public GameObject popup;
	[SerializeField]
	GameObject ticketsPanel;
	[SerializeField]
	Camera playerCamera;

	float ZOOM_OUT_ANGLE = 100f;
	float ZOOM_IN_ANGLE = 59f;
	int TICKET_PRICE = 100;
	float ANIM_DURATION = 3f;

	public void ClosePanel() {
		ticketsPanel.SetActive(false);
	}

	public void OpenPanel() {
		AudioSource audio = GameObject.Find("AudioFerry").GetComponent<AudioSource>();
		audio.Play();
		score.GetComponent<Text>().text = player.GetComponent<NewPlayer>().GetTotalScore().ToString();
		ticketsPanel.SetActive(true);
	}

	public void GetTicket() {
		var total = player.GetComponent<NewPlayer>().GetTotalScore();
		if (total >= TICKET_PRICE) {
			player.GetComponent<NewPlayer>().AddTotalScore(-TICKET_PRICE);
			popup.SetActive(true);
		} else {
			Debug.Log("points not enough");
		}
	}

	public void Comfirmed() {
		ClosePanel();
		var ferry = GameObject.Find("Ferry");
		// move character to the ferry
		player.transform.DOMove(new Vector3(-479.2394f, 48f, 0f), ANIM_DURATION).OnComplete(() => {
			player.transform.parent = ferry.transform;
			player.GetComponent<SpriteRenderer>().enabled = false;

			// Zoom out camera view
			playerCamera.GetComponent<Camera>().DOFieldOfView(ZOOM_OUT_ANGLE, ANIM_DURATION);
			
			var seq = DOTween.Sequence ();  
			// move ferry 
			seq.Append(ferry.GetComponent<DOTweenPath>().tween)
				.OnComplete(() => {
					// move character to the target island
					player.GetComponent<SpriteRenderer>().enabled = true;
					player.transform.parent = null;
					playerCamera.GetComponent<Camera>().DOFieldOfView(ZOOM_IN_ANGLE, ANIM_DURATION);
					player.transform.DOMove(new Vector3(830f, -30f, 0f), ANIM_DURATION).OnComplete(() => {
						player.GetComponent<NewPlayer>().SaveCurrentIsland("B");
						player.GetComponent<NewPlayer>().SetUpMiniMap();
						player.GetComponent<NewPlayer>().SaveCoordinate();
						player.GetComponent<NewPlayer>().PlayBGM();
					});
				});
		});
	}

	public void NotComfirmed() {
		popup.SetActive(false);
	}
}
