<script setup>
import Nav from "@/components/Nav.vue";
import Footer from "@/components/Footer.vue";
import Tooltip from "@/components/common/Tooltip.vue";
import skinData from "@/assets/data/skin.json";
import { ref, onBeforeMount } from "vue";
import { myNFTs } from "@/api/nft";
import { putEquipSkin, getEquippedSkin, getSkinList } from "@/api/skin";
import { postMint } from "@/api/contract";
import { useSkinStore } from "@/stores/skin";
import { useUserStore } from "@/stores/user";
import { useNftStore } from "@/stores/nft";

const sStore = useSkinStore();
const uStore = useUserStore();
const nStore = useNftStore();

const currentSkin = ref(3);
const spacing = ref(0);


const defaultUrl = "none";
const defaultDescription = "잠금"
const bear = ref({
  nft: Array.from({ length: 15 }, () => ({
    imageUrl: defaultUrl,
    description: defaultDescription,
  })),
});
const rabbit = ref({
  nft: Array.from({ length: 15 }, () => ({
    imageUrl: defaultUrl,
    description: defaultDescription,
  })),
});
const nftData = ref([
  {
    id: 0,
    description: "",
    attributes: {},
    imageUrl: "",
    metadataUri: "",
  }
]);

onBeforeMount(async () => {
  sStore.userSkin["choice"] = "bear";
  try {
    const response = await getEquippedSkin(uStore.user.data.userId);
    if (response.data[0].attributes.character === "bear") {
      sStore.userSkin.selectedBearMetadata = sStore.userSkin.bearMetadata = response.data[0].id;
      sStore.userSkin.selectedRabbitMetadata = sStore.userSkin.rabbitMetadata = response.data[1].id;
    } else {
      sStore.userSkin.selectedRabbitMetadata = sStore.userSkin.rabbitMetadata = response.data[0].id;
      sStore.userSkin.selectedBearMetadata = sStore.userSkin.bearMetadata = response.data[1].id;
    }
  } catch (error) {
    console.error(error);
  }

  try {
    const response = await getSkinList(uStore.user.data.userId);
    nftData.value = response.data;
    nftData.value = prepareNftData(response.data);
    filterNftData(nftData);
    sStore.userBearSkin = bear.value.nft;
    sStore.userRabbitSkin = rabbit.value.nft;
    sStore.userSkin.selectedBearMetadata = sStore.userBearSkin[currentSkin.value].id;
  } catch (error) {
    console.error(error);
  }

  if (!uStore.user.data.metamaskToken) {
    return;
  }

  try {
    const response = await myNFTs(uStore.user.data.address, uStore.user.data.metamaskToken);
    nStore.userNft = response.data;
  } catch (error) {
    Swal.fire({
      icon: "error",
      title: "NFT를 불러오는데 실패했습니다. 지갑 연동 상태를 확인해주세요.",
    });
    uStore.user.data.metamaskToken = "";
    uStore.user.data.address = "";
    console.error(error);
  }
})


function skinScale(idx) {
  // console.log(idx);

  if (idx == currentSkin.value) {
    return "skin-center";
  } else if (idx == currentSkin.value + 1) {
    return "skin-right";
  } else if (idx == currentSkin.value - 1) {
    return "skin-left";
  } else {
    return "";
  }
}

function nextBtn() {
  currentSkin.value += 1;
  if (currentSkin.value === 15) {
    currentSkin.value = 14;

  } else {
    spacing.value -= 150;
  }
  sStore.userSkin.selectedBearMetadata = sStore.userBearSkin[currentSkin.value].id;
  sStore.userSkin.selectedRabbitMetadata = sStore.userRabbitSkin[currentSkin.value].id;
}

function prevBtn() {
  currentSkin.value -= 1;
  console.log(currentSkin.value);
  if (currentSkin.value === -1) {
    currentSkin.value = 0;
  } else {
    spacing.value += 150;
  }
  sStore.userSkin.selectedBearMetadata = sStore.userBearSkin[currentSkin.value].id;
  sStore.userSkin.selectedRabbitMetadata = sStore.userRabbitSkin[currentSkin.value].id;
}

function positionCalc(idx) {
  let val = 40 + (idx - 1) * 150;
  if (idx == currentSkin.value) {
    val += 20;
  } else if (idx == currentSkin.value + 1) {
    val += 40;
  } else if (idx == currentSkin.value + 2) {
    val += 40;
  } else if (idx == currentSkin.value - 1) {
  } else if (idx > currentSkin.value + 2) {
    val += 150;
  }
  return spacing.value + val + "px";
}

function prepareNftData(responseData) {
  return responseData.map((nft) => {
    return {
      id: nft.id,
      description: nft.description,
      attributes: nft.attributes,
      imageUrl: nft.image_url,
      metadataUri: nft.metadata_uri,
      name: nft.name,
    };
  });
}

function filterNftData(nftData) {
  let bearIndex = 0
  let rabbitIndex = 0
  nftData.value.forEach((nft) => {
    if (nft.attributes.character.toLowerCase() === "bear") {
      bear.value.nft[bearIndex] = nft;
      bearIndex++;
    } else {
      rabbit.value.nft[rabbitIndex] = nft;
      rabbitIndex++;
    }
  });
}

function choiceCharacter(character) {
  sStore.userSkin.choice = character;
}

function imgUrl(nft) {
  if (!nft.imageUrl) {
    return "";
  }
  if (nft.imageUrl === "none") {
    return new URL(`/src/assets/collection/nft/lock-nft.svg`, import.meta.url).href;
  }
  const url = nft.imageUrl;
  return new URL(`${url}`, import.meta.url).href;
}

function changeSkin(selectedSkin) {
  if (!selectedSkin.attributes) {
    Swal.fire({
      icon: "error",
      title: "NFT를 가지고 있지 않습니다",
      text: "스토리를 클리어하면서 NFT를 수집해보세요!",
    });
    return;
  }
  if (sStore.userSkin.choice === "bear") {
    sStore.userSkin.selectedBearMetadata = selectedSkin.id;
  } else if (sStore.userSkin.choice === "rabbit") {
    sStore.userSkin.selectedRabbitMetadata = selectedSkin.id;
  }
}

async function equipSkin() {
  let choice = "";
  let selectedMetadata = "";

  console.log("inner equipSkin")

  if (sStore.userSkin.choice === "bear") {
    choice = "bear";
    selectedMetadata = sStore.userSkin.selectedBearMetadata;
  } else if (sStore.userSkin.choice === "rabbit") {
    choice = "rabbit";
    selectedMetadata = sStore.userSkin.selectedRabbitMetadata;
  }
  try {
    const response = await putEquipSkin(uStore.user.data.userId, choice, selectedMetadata);
    if (response.status === 200) {
      if (choice === "bear") {
        sStore.userSkin.bearMetadata = sStore.userSkin.selectedBearMetadata;
      } else if (choice === "rabbit") {
        sStore.userSkin.rabbitMetadata = sStore.userSkin.selectedRabbitMetadata;
      }
      Swal.fire({
        icon: "success",
        title: "성공",
        text: "스킨이 변경되었습니다",
      });
    }
  } catch (error) {
    Swal.fire({
      icon: "error",
      title: "실패",
      text: "스킨 변경에 실패했습니다",
    })
    console.error(error);
  }
}

async function mint() {
  let tokenURI = "";
  if (sStore.userSkin.choice === "bear") {
    tokenURI = sStore.userBearSkin.filter((skin) => skin.attributes && skin.id == sStore.userSkin.selectedBearMetadata)[0].metadataUri;
  } else if (sStore.userSkin.choice === "rabbit") {
    tokenURI = sStore.userRabbitSkin.filter((skin) => skin.attributes && skin.id == sStore.userSkin.selectedRabbitMetadata)[0].metadataUri;
  }
  try {
    Swal.fire({
      title: "NFT 발행 중",
      text: "잠시만 기다려주세요. 네트워크 상태에 따라 시간이 걸릴 수 있습니다.",
      showConfirmButton: false,
      allowOutsideClick: false,
      allowEscapeKey: false,
      allowEnterKey: false,
      showCloseButton: false,
      didOpen: () => {
        Swal.showLoading()
      }
    })
    const response = await postMint(uStore.user.data.userId, uStore.user.data.address, tokenURI);
    if (response.status === 200) {
      Swal.close();
      Swal.fire({
        icon: "success",
        title: "성공",
        text: "NFT가 발행되었습니다",
      });
      window.location.reload();
    }
  } catch (error) {
    Swal.close();
    Swal.fire({
      icon: "error",
      title: "실패",
      text: "NFT 발행에 실패했습니다",
    });
    console.error(error);
  }
}

function equipSkinCheck() {
  if (sStore.userSkin.choice === "bear") {
    if (sStore.userBearSkin[currentSkin.value].description === "잠금") {
      return false;
    }
  } else if (sStore.userSkin.choice === "rabbit") {
    if (sStore.userRabbitSkin[currentSkin.value].id === "잠금") {
      return false;
    }
  }
  if (sStore.userSkin.choice === "bear") {
    return sStore.userSkin.selectedBearMetadata !== sStore.userSkin.bearMetadata;
  } else if (sStore.userSkin.choice === "rabbit") {
    return sStore.userSkin.selectedRabbitMetadata !== sStore.userSkin.rabbitMetadata;
  }
}

function hasNFTCheck() {
  if (!uStore.user.data.metamaskToken || nStore.userNft.length === 0) {
    return
  }
  if (sStore.userSkin.choice === "bear") {
    return nStore.userNft.some((nft) => nft.attributes.character === 'bear' && nft.id == sStore.userSkin.selectedBearMetadata)
  } else if (sStore.userSkin.choice === "rabbit") {
    return nStore.userNft.some((nft) => nft.attributes.character === 'rabbit' && nft.id == sStore.userSkin.selectedRabbitMetadata)
  }
}

function hasNFTCheck2(id) {
  if (!uStore.user.data.metamaskToken || nStore.userNft.length === 0) {
    return false
  }
  return nStore.userNft.some((nft) => nft.id == id)
}

function openNftInfo(id) {
  Swal.fire({
    title: "NFT 정보",
    text: "NFT 정보를 확인하시겠습니까?",
    showCancelButton: true,
    confirmButtonText: "확인",
    cancelButtonText: "취소",
  }).then((result) => {
    if (result.isConfirmed) {
      const url = `/nft?metadataId=${id}`;
      window.open(url, "_blank", `width=400, height=300`);
    }
  });
}
</script>

<template>
  <div>
    <Nav />
    <div class="main-con box-md">
      <div class="character-con">
        <button v-if="sStore.userSkin.choice === 'bear'" class="character-btn bitbit selected"
          @click="choiceCharacter('bear')">곰</button>
        <button v-else class="character-btn bitbit" @click="choiceCharacter('bear')">곰</button>
        <button v-if="sStore.userSkin.choice === 'rabbit'" class="character-btn bitbit selected"
          @click="choiceCharacter('rabbit')">토끼</button>
        <button v-else class="character-btn bitbit" @click="choiceCharacter('rabbit')">토끼</button>
      </div>
      <div class="skin-con box-col">
        <div v-if="sStore.userSkin.choice === 'bear'" class="skin-con">
          <img :src="imgUrl(sStore.userBearSkin[currentSkin])" alt="곰1" class="main-skin" />
        </div>
        <div v-else-if="sStore.userSkin.choice === 'rabbit'" class="skin-con">
          <img :src="imgUrl(sStore.userRabbitSkin[currentSkin])" alt="토끼1" class="main-skin" />
        </div>
        <div v-if="sStore.userSkin.choice === 'bear'" class="skin-name box-md bitbit">
          {{ sStore.userBearSkin[currentSkin].description }}
        </div>
        <div v-else-if="sStore.userSkin.choice === 'rabbit'" class="skin-name box-md bitbit">
          {{ sStore.userRabbitSkin[currentSkin].description }}
        </div>
      </div>
      <div class="skin-list flex-align">
        <button class="skin-btn btn bitbit" @click="prevBtn">&lt;
        </button>
        <div class="skins flex-align">
          <div v-if="sStore.userSkin.choice === 'bear'" class="skin-list">
            <div class="image-con">
              <div v-for="(skin, num) in sStore.userBearSkin" :key="num">
                <img :src="imgUrl(skin)" :alt="skin.name" class="skin" :class="skinScale(num)"
                  :style="{ left: positionCalc(num) }">
                </img>
                <div v-if="sStore.userBearSkin[num].id == sStore.userSkin.bearMetadata" class="skin-badge"
                  :class="skinScale(num)" :style="{ left: `calc(${positionCalc(num)} - 3%)` }">⭐</div>
                <div v-if="uStore.user.data.metamaskToken && hasNFTCheck2(sStore.userBearSkin[num].id)"
                  class="nft-badge" :class="skinScale(num)" :style="{ left: `calc(${positionCalc(num)} + 10.5%)` }"
                  @click="openNftInfo(skin.id)">NFT
                </div>
              </div>
            </div>
          </div>
          <div v-else-if="sStore.userSkin.choice === 'rabbit'" class="skin-list">
            <div class="image-con">
              <div v-for="(skin, num) in sStore.userRabbitSkin" :key="num">
                <img :src="imgUrl(skin)" :alt="skin.name" class="skin" :class="skinScale(num)"
                  :style="{ left: positionCalc(num) }">
                </img>
                <div v-if="sStore.userRabbitSkin[num].id == sStore.userSkin.rabbitMetadata" class="skin-badge"
                  :class="skinScale(num)" :style="{ left: `calc(${positionCalc(num)} - 3%)` }">⭐</div>
                <div v-if="uStore.user.data.metamaskToken && hasNFTCheck2(sStore.userRabbitSkin[num].id)"
                  class="nft-badge" :class="skinScale(num)" :style="{ left: `calc(${positionCalc(num)} + 10.5%)` }"
                  @click="openNftInfo(skin.id)">NFT
                </div>
              </div>
            </div>
          </div>
        </div>
        <button class="skin-btn btn bitbit" @click="nextBtn">></button>
      </div>
      <div class="btn-con">
        <div class="tooltip">
          <div v-if="uStore.user.data.metamaskToken && !hasNFTCheck()">
            <button class="nft-btn btn bitbit" @click="mint()">NFT
              발행</button>
          </div>
          <div v-else>
            <button class="nft-btn btn bitbit disable">NFT 발행</button>
            <Tooltip v-if="!uStore.user.data.metamaskToken" message="MetaMask를 연동해주세요." />
            <Tooltip v-else message="이미 발행한 NFT입니다." />
          </div>
        </div>
        <button v-if="equipSkinCheck()" class="select-btn btn bitbit" @click="equipSkin()">선택하기</button>
        <button v-else class="select-btn btn bitbit disable">선택하기</button>
      </div>
    </div>
    <Footer />
  </div>
</template>

<style scoped>
.main-con {
  height: 950px;
  background-color: black;
  padding-top: 50px;
  flex-direction: column;
  justify-content: space-around;
}

.select-btn,
.nft-btn {
  position: absolute;
  bottom: -130px;
  transition: all 0.2s ease-in;

  padding: 10px 20px;
}

.btn-con {
  width: 300px;
  position: absolute;
  bottom: -90px;
  background-color: white;
}

.select-btn:hover,
.nft-btn:hover {
  box-shadow: 0px 0px 0px 5px #aba4f7;
}

.skin-con {
  width: 600px;
  height: 450px;

  background-color: rgb(0, 0, 0);
  position: relative;
}

.main-skin {
  width: 100%;
  height: 100%;
  object-fit: cover;
  /* 이미지 비율을 유지하면서 부모 요소를 꽉 채우기 */
}

.skin-name {
  color: white;
  margin-top: 20px;

  font-size: 25px;
}

.skin-list {
  width: 60%;
  margin-bottom: 50px;
}

.skins {
  width: 100%;
  height: 320px;
  /* background-color: brown; */
  justify-content: space-between;
  position: relative;
  overflow: hidden;
}

.skin-center {
  scale: 1.4;
}

.skin-left,
.skin-right {
  scale: 1.2;
}

.skin {
  transition: all 0.5s ease-in-out;
  width: 12%;
  position: absolute;
}

.skin-badge {
  transition: all 0.5s ease-in-out;
  position: absolute;
  top: 36%;
  font-size: 18px;
  /* background-color: gold; */
  color: white;
  /* border-radius: 50%; */
  /* font-size: 12px; */
}

.nft-badge {
  transition: all 0.5s ease-in-out;
  position: absolute;
  top: 34%;
  left: 56.5%;
  background-color: #4caf50;
  color: white;
  padding: 5px;
  border-radius: 5px;
  font-size: 10px;
}

.btn {
  background-color: #1f1a59;
  color: white;
  font-size: 25px;
  text-align: center;
  border-radius: 10px;
  border-width: 5px;
  border-color: #1c165c;
}

.skin-btn {
  width: 50px;
  height: 50px;
}

.select-btn,
.nft-btn {
  border-width: 1px;
}

.select-btn {
  right: 0px;
}

.disable {
  transform: scale(1) !important;
  cursor: not-allowed !important;
  filter: brightness(0.7);
  opacity: 0.7 !important;
}

.tooltip {
  position: relative;
  /* display: inline-block; */
}

.character-con {
  width: 100%;
  display: flex;
  justify-content: center;
}

.character-btn {
  width: 100px;
  margin-left: 10px;
  background-color: hsl(235, 55%, 35%);
  color: white;
  font-size: 25px;
  text-align: center;
  border-radius: 10px;
  border-width: 5px;
  border-color: hsl(235, 61%, 34%);
  border-width: 3px;
}

.selected {
  background-color: hsl(235, 55%, 25%);
  box-shadow: inset 2px 2px 5px rgba(0, 0, 0, 0.5)
}
</style>
