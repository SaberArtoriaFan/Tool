import { wrapper } from "./Wrapper";

class _UMNativeSdk {
    private rewardSuccessCallback: Function
    private rewardFailCallback: Function
    private rewardErrorCallback: Function

    private SDKName: string = "com.qxqrj.ad.AdSDK";

    constructor() {
        window["UMNativeSdk"] = this;
    }
    /**
     *播放视频
     * @param {Function} success 播放完成回调
     * @param {Function} [fail=null]    播放未完成回调
     * @param {Function} [error=null]   播放失败回调
     * @memberof _UMSdk
     */
    ShowVideo(success: Function, str: string, fail: Function = null, error: Function = null) {
        console.log("Saber播放视频")
        this.CallJava("ShowVideo", '(Ljava/lang/String;)V', str);
        this.rewardSuccessCallback = success
        this.rewardFailCallback = fail
        this.rewardErrorCallback = error
    }

    AfterPlayVideo(status: string) {
        console.log("AfterPlayVideo", status)
        if (status == "success") {
            if (this.rewardSuccessCallback) {
                console.log("Saber触发 播放视频成功事件");
                this.rewardSuccessCallback()
            }
        } else if (status == "fail") {
            if (this.rewardFailCallback) {
                console.log("Saber触发 播放视频失败事件");
                this.rewardFailCallback()
            }
        }
    }

    /**
     *展示banner
     * @memberof _UMSdk
     */
    ShowBanner() {
        console.log("展示Banner");
        this.CallJava("ShowBanner", '(Ljava/lang/String;)V', "down");
    }


    public AndroidSDKName(str: string) {
        this.SDKName = str;
    }

    /**
     *关闭banner
     * @memberof _UMSdk
     */
    CloseBanner() {
        this.CallJava("CloseBanner", '()V');
    }

    /**
     *展示插屏
     *
     * @memberof _UMSdk
     */
    ShowInterstitial(type: string = "") {
        console.log("Saber展示插屏", type, "-------------");
        // audioManager.setVolume(0);
        // tips.show(`当前通关:${levelManager.levelInfo.levelId},调用ShowInterstitial`)
        this.CallJava("ShowInterstitial", '(Ljava/lang/String;)V', type);
    }

    AfterPlayInterstitial(type: string) {
        // audioManager.setVolume(1);
        if (type == "success") {

        }
        console.log("AfterPlayInterstitial", type);
    }

    LogEvent(name: string) {
        console.log("埋点事件名字" + name)
        this.CallJava("LogEvent", '(Ljava/lang/String;)V', name);
    }

    LogEvent2(name: string, str) {
        console.log("埋点事件名字" + name, "埋点事件参数" + str)
        this.CallJava("LogEvent", '(Ljava/lang/String;Ljava/lang/String;)V', name, str);
    }

    GetOnlineValue(key: string): string {
        return this.CallJava("GetOnlineValue", '(Ljava/lang/String;)Ljava/lang/String;', key);
    }

    PhoneShake(millisecond: number) {
        this.CallJava("PhoneShake", '(I)V', millisecond);
    }

    SetConfigVersion(version: string) {
        this.CallJava("SetConfigVersion", '(Ljava/lang/String;)V', version);
    }

    FetchConfig() {
        this.CallJava("FetchConfig", '()V');
    }

    AfterFetchConfig(status: string) {
        console.log("AfterFetchConfig" + status)
        if (status == "success") {
            wrapper.isAfterFetchConfigSucc = true;
        }
    }

    //显示谷歌评分
    ShowGooglePlayeAssess() {
        console.log("谷歌评分展示");

        this.CallJava("showGooglePlayAssess", '()V')
    }

    /**
     *
     * @param className 类名 com/johnny/test/WxApiHelper
     * @param staticMethodName 静态方法名
     * @param methodSignature 参数标识 ()V
     * int	I
        float	F
        boolean	Z
        String	Ljava/lang/String;
    * @param parameters 参数
    */
    CallJava(staticMethodName: string, methodSignature: string, ...parameters) {
        console.log("CallJava", staticMethodName, methodSignature, ...parameters)
        if (cc.sys.platform == cc.sys.ANDROID) {
            return jsb.reflection.callStaticMethod(this.SDKName, staticMethodName, methodSignature, ...parameters)
        } else {
            return ""
        }
    }
}
export const UMNativeSdk = new _UMNativeSdk()
