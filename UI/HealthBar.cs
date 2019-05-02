using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EF RID: 1519
	[RequireComponent(typeof(RectTransform))]
	public class HealthBar : MonoBehaviour
	{
		// Token: 0x06002238 RID: 8760 RVA: 0x00018E54 File Offset: 0x00017054
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.healthbarScale = 1f;
			this.fillImage = this.fillRectTransform.GetComponent<Image>();
			this.originalFillColor = this.fillImage.color;
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x00018E8F File Offset: 0x0001708F
		private void Start()
		{
			this.UpdateHealthbar(0f);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x00018E9C File Offset: 0x0001709C
		public void Update()
		{
			this.UpdateHealthbar(Time.deltaTime);
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x000A44B8 File Offset: 0x000A26B8
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
					this.frozenCullThresholdRectTransform.gameObject.SetActive(this.source.isInFrozenState);
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

		// Token: 0x0600223C RID: 8764 RVA: 0x00018EA9 File Offset: 0x000170A9
		public static Color GetCriticallyHurtColor()
		{
			if (Mathf.FloorToInt(Time.fixedTime * 10f) % 2 != 0)
			{
				return ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
			}
			return Color.white;
		}

		// Token: 0x040024BD RID: 9405
		public HealthComponent source;

		// Token: 0x040024BE RID: 9406
		public RectTransform fillRectTransform;

		// Token: 0x040024BF RID: 9407
		public RectTransform shieldFillRectTransform;

		// Token: 0x040024C0 RID: 9408
		public RectTransform delayfillRectTransform;

		// Token: 0x040024C1 RID: 9409
		public RectTransform flashRectTransform;

		// Token: 0x040024C2 RID: 9410
		public RectTransform eliteBackdropRectTransform;

		// Token: 0x040024C3 RID: 9411
		public RectTransform frozenCullThresholdRectTransform;

		// Token: 0x040024C4 RID: 9412
		public TextMeshProUGUI currentHealthText;

		// Token: 0x040024C5 RID: 9413
		public TextMeshProUGUI fullHealthText;

		// Token: 0x040024C6 RID: 9414
		public Image criticallyHurtImage;

		// Token: 0x040024C7 RID: 9415
		public Image deadImage;

		// Token: 0x040024C8 RID: 9416
		public float maxLastHitTimer = 1f;

		// Token: 0x040024C9 RID: 9417
		public bool scaleHealthbarWidth;

		// Token: 0x040024CA RID: 9418
		public float minHealthbarWidth;

		// Token: 0x040024CB RID: 9419
		public float maxHealthbarWidth;

		// Token: 0x040024CC RID: 9420
		public float minHealthbarHealth;

		// Token: 0x040024CD RID: 9421
		public float maxHealthbarHealth;

		// Token: 0x040024CE RID: 9422
		private float displayStringCurrentHealth;

		// Token: 0x040024CF RID: 9423
		private float displayStringFullHealth;

		// Token: 0x040024D0 RID: 9424
		private RectTransform rectTransform;

		// Token: 0x040024D1 RID: 9425
		private float lastHitTimer;

		// Token: 0x040024D2 RID: 9426
		private float cachedFractionalValue = 1f;

		// Token: 0x040024D3 RID: 9427
		private bool hasCachedInitialValue;

		// Token: 0x040024D4 RID: 9428
		private float healthbarScale = 1f;

		// Token: 0x040024D5 RID: 9429
		private Image fillImage;

		// Token: 0x040024D6 RID: 9430
		private Color originalFillColor;

		// Token: 0x040024D7 RID: 9431
		public static float criticallyHurtThreshold = 0.3f;

		// Token: 0x040024D8 RID: 9432
		private float theta;

		// Token: 0x040024D9 RID: 9433
		private float healthFractionVelocity;
	}
}
