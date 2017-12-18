declare namespace abp {

    namespace signalr {

        let autoConnect: boolean;

        let qs: string;

        let url: string;

        function connect(): any;

        namespace hubs {

            let common: any;

        }

    }

}