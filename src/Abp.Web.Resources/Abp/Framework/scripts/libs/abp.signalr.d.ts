declare namespace abp {

    namespace signalr {

        let autoConnect: boolean;

        let qs: string;

        let remoteServiceBaseUrl: string;

        let url: string;

        function connect(): any;

        function startConnection(url: string, configureConnection: Function): Promise<any>;

        namespace hubs {

            let common: any;

        }

    }

}