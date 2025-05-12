import * as Playroom from "playroomkit";
window.Playroom = Playroom;

Playroom.getDiscordClient()?.commands.startPurchase({sku_id: "playroomkit"}).then((value) => {
})
