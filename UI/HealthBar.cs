using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DD RID: 1501
	[RequireComponent(typeof(RectTransform))]
	public class HealthBar : MonoBehaviour
	{
		// Token: 0x060021A7 RID: 8615 RVA: 0x0001875A File Offset: 0x0001695A
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.healthbarScale = 1f;
			this.fillImage = this.fillRectTransform.GetComponent<Image>();
			this.originalFillColor = this.fillImage.color;
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x00018795 File Offset: 0x00016995
		private void Start()
		{
			this.UpdateHealthbar(0f);
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x000187A2 File Offset: 0x000169A2
		public void Update()
		{
			this.UpdateHealthbar(Time.deltaTime);
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x000A2EE4 File Offset: 0x000A10E4
		private void UpdateHealthbar(float deltaTime)
		{
			float num = 0f;
			float num2 = 1f;
			float num3 = 1f;
			if (this.source)
			{
				CharacterBody component = this.source.GetComponent<CharacterBody>();
				if (component)
				{
					float num4 = component.CalcLunarDaggerPower();
					num3 /= num4;
				}
				float fullHealth = this.source.fullHealth;
				float f = this.source.health + this.source.shield;
				float num5 = this.source.fullHealth + this.source.fullShield;
				num = Mathf.Clamp01(this.source.health / num5 * num3);
				float num6 = Mathf.Clamp01(this.source.shield / num5 * num3);
				if (!this.hasCachedInitialValue)
				{
					this.cachedFractionalValue = num;
					this.hasCachedInitialValue = true;
				}
				if (this.eliteBackdropRectTransform)
				{
					if (component.equipmentSlot && EliteCatalog.IsEquipmentElite(component.equipmentSlot.equipmentIndex) != EliteIndex.None)
					{
						num2 += 1f;
						this.eliteBackdropRectTransform.gameObject.SetActive(true);
					}
					else
					{
						this.eliteBackdropRectTransform.gameObject.SetActive(false);
					}
				}
				if (this.frozenCullThresholdRectTransform)
				{
					this.frozenCullThresholdRectTransform.gameObject.SetActive(this.source.isFrozen);
				}
				bool active = false;
				if (this.source.fullShield > 0f)
				{
					active = true;
				}
				this.shieldFillRectTransform.gameObject.SetActive(active);
				if (this.scaleHealthbarWidth && component)
				{
					float num7 = Util.Remap(Mathf.Clamp((component.baseMaxHealth + component.baseMaxShield) * num2, 0f, this.maxHealthbarHealth), this.minHealthbarHealth, this.maxHealthbarHealth, this.minHealthbarWidth, this.maxHealthbarWidth);
					this.healthbarScale = num7 / this.minHealthbarWidth;
					this.rectTransform.sizeDelta = new Vector2(num7, this.rectTransform.sizeDelta.y);
				}
				Color color = this.originalFillColor;
				CharacterMaster master = component.master;
				if (master && (master.isBoss || master.inventory.GetItemCount(ItemIndex.Infusion) > 0))
				{
					color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
				}
				this.fillImage.color = color;
				if (this.fillRectTransform)
				{
					this.fillRectTransform.anchorMin = new Vector2(0f, 0f);
					this.fillRectTransform.anchorMax = new Vector2(num, 1f);
					this.fillRectTransform.anchoredPosition = Vector2.zero;
					this.fillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.shieldFillRectTransform)
				{
					this.shieldFillRectTransform.anchorMin = new Vector2(num, 0f);
					this.shieldFillRectTransform.anchorMax = new Vector2(num + num6, 1f);
					this.shieldFillRectTransform.anchoredPosition = Vector2.zero;
					this.shieldFillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.delayfillRectTransform)
				{
					this.delayfillRectTransform.anchorMin = new Vector2(0f, 0f);
					this.delayfillRectTransform.anchorMax = new Vector2(this.cachedFractionalValue, 1f);
					this.delayfillRectTransform.anchoredPosition = Vector2.zero;
					this.delayfillRectTransform.sizeDelta = new Vector2(1f, 1f);
				}
				if (this.flashRectTransform)
				{
					this.flashRectTransform.anchorMin = new Vector2(0f, 0f);
					this.flashRectTransform.anchorMax = new Vector2(num, 1f);
					float num8 = 1f - num;
					float num9 = 2f * num8;
					this.theta += deltaTime * num9;
					if (this.theta > 1f)
					{
						this.theta -= this.theta - this.theta % 1f;
					}
					float num10 = 1f - Mathf.Cos(this.theta * 3.14159274f * 0.5f);
					this.flashRectTransform.sizeDelta = new Vector2(num10 * 20f * num8, num10 * 20f * num8);
					Image component2 = this.flashRectTransform.GetComponent<Image>();
					if (component2)
					{
						Color color2 = component2.color;
						color2.a = (1f - num10) * num8 * 0.7f;
						component2.color = color2;
					}
				}
				if (this.currentHealthText)
				{
					float num11 = Mathf.Ceil(f);
					if (num11 != this.displayStringCurrentHealth)
					{
						this.displayStringCurrentHealth = num11;
						this.currentHealthText.text = num11.ToString();
					}
				}
				if (this.fullHealthText)
				{
					float num12 = Mathf.Ceil(fullHealth);
					if (num12 != this.displayStringFullHealth)
					{
						this.displayStringFullHealth = num12;
						this.fullHealthText.text = num12.ToString();
					}
				}
				if (this.criticallyHurtImage)
				{
					if (num + num6 < HealthBar.criticallyHurtThreshold && this.source.alive)
					{
						this.criticallyHurtImage.enabled = true;
						this.criticallyHurtImage.color = HealthBar.GetCriticallyHurtColor();
						this.fillImage.color = HealthBar.GetCriticallyHurtColor();
					}
					else
					{
						this.criticallyHurtImage.enabled = false;
					}
				}
				if (this.deadImage)
				{
					this.deadImage.enabled = !this.source.alive;
				}
			}
			this.cachedFractionalValue = Mathf.SmoothDamp(this.cachedFractionalValue, num, ref this.healthFractionVelocity, 0.05f, float.PositiveInfinity, deltaTime);
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x000187AF File Offset: 0x000169AF
		public static Color GetCriticallyHurtColor()
		{
			if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 != 0)
			{
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			}
			return Color.white;
		}

		// Token: 0x04002469 RID: 9321
		public HealthComponent source;

		// Token: 0x0400246A RID: 9322
		public RectTransform fillRectTransform;

		// Token: 0x0400246B RID: 9323
		public RectTransform shieldFillRectTransform;

		// Token: 0x0400246C RID: 9324
		public RectTransform delayfillRectTransform;

		// Token: 0x0400246D RID: 9325
		public RectTransform flashRectTransform;

		// Token: 0x0400246E RID: 9326
		public RectTransform eliteBackdropRectTransform;

		// Token: 0x0400246F RID: 9327
		public RectTransform frozenCullThresholdRectTransform;

		// Token: 0x04002470 RID: 9328
		public TextMeshProUGUI currentHealthText;

		// Token: 0x04002471 RID: 9329
		public TextMeshProUGUI fullHealthText;

		// Token: 0x04002472 RID: 9330
		public Image criticallyHurtImage;

		// Token: 0x04002473 RID: 9331
		public Image deadImage;

		// Token: 0x04002474 RID: 9332
		public float maxLastHitTimer = 1f;

		// Token: 0x04002475 RID: 9333
		public bool scaleHealthbarWidth;

		// Token: 0x04002476 RID: 9334
		public float minHealthbarWidth;

		// Token: 0x04002477 RID: 9335
		public float maxHealthbarWidth;

		// Token: 0x04002478 RID: 9336
		public float minHealthbarHealth;

		// Token: 0x04002479 RID: 9337
		public float maxHealthbarHealth;

		// Token: 0x0400247A RID: 9338
		private float displayStringCurrentHealth;

		// Token: 0x0400247B RID: 9339
		private float displayStringFullHealth;

		// Token: 0x0400247C RID: 9340
		private RectTransform rectTransform;

		// Token: 0x0400247D RID: 9341
		private float lastHitTimer;

		// Token: 0x0400247E RID: 9342
		private float cachedFractionalValue = 1f;

		// Token: 0x0400247F RID: 9343
		private bool hasCachedInitialValue;

		// Token: 0x04002480 RID: 9344
		private float healthbarScale = 1f;

		// Token: 0x04002481 RID: 9345
		private Image fillImage;

		// Token: 0x04002482 RID: 9346
		private Color originalFillColor;

		// Token: 0x04002483 RID: 9347
		public static float criticallyHurtThreshold = 0.3f;

		// Token: 0x04002484 RID: 9348
		private float theta;

		// Token: 0x04002485 RID: 9349
		private float healthFractionVelocity;
	}
}
