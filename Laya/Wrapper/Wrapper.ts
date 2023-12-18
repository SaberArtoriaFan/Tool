import { UMNativeSdk } from "./UMNativeSdk";

/**
 * 包装
 */
class Wrapper {

    private _isAfterFetchConfigSucc: boolean = false;
    public get isAfterFetchConfigSucc(): boolean { return this._isAfterFetchConfigSucc; }
    public set isAfterFetchConfigSucc(value: boolean) { this._isAfterFetchConfigSucc = value; }



    public ShowVideo(success: Function, str: string, fail: Function = null, error: Function = null): void {
        // if (cc.sys.isBrowser) {
        //     success()
        //     return
        // }
        console.log("展示视频")
        UMNativeSdk.ShowVideo(success, str, fail, error);
    }


    public AfterPlayVideo(status: string): void {
        UMNativeSdk.AfterPlayVideo(status);
    }


    public ShowBanner(): void {
        console.log("-----------------------------------------");
        console.log("展示Banner");
        UMNativeSdk.ShowBanner();
    }

    public CloseBanner(): void {
        UMNativeSdk.CloseBanner();
    }

    public ShowInterstitial(type: string = "time"): void {
        UMNativeSdk.ShowInterstitial(type);
    }

    public AfterPlayInterstitial(type: string): void {
        UMNativeSdk.AfterPlayInterstitial(type);
    }

    public IsVideoLoaded(): boolean {
        return false;
    }

    public LogEvent(name: string): void {
        UMNativeSdk.LogEvent(name);
    }
    public LogEvent2(name: string, str): void {
        UMNativeSdk.LogEvent2(name, str);
    }

    public GetOnlineValue(key: string): string {
        return UMNativeSdk.GetOnlineValue(key);
    }

    public PhoneShake(millisecond: number = 0.5) {
        UMNativeSdk.PhoneShake(millisecond);
    }

    public SetConfigVersion(version: string) {
        UMNativeSdk.SetConfigVersion(version);
    }

    public FetchConfig() {
        UMNativeSdk.FetchConfig();
    }
    public ShowGooglePlayAssess() {
        UMNativeSdk.ShowGooglePlayeAssess();
    }


    public AfterFetchConfig(status: string) {
        UMNativeSdk.AfterFetchConfig(status);
    }


    public CallJava(staticMethodName: string, methodSignature: string, ...parameters) {
        UMNativeSdk.CallJava(staticMethodName, methodSignature, parameters);
    }




}
export const wrapper: Wrapper = new Wrapper();