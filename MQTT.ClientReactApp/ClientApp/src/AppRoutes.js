import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Home } from "./components/Home";
import { UserSession } from "./components/UserSession";
import { BackChannelArea } from "./components/BackChannelArea";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
   },
   {
       path: '/user-session',
       element: <UserSession />
    },
    {
        path: '/back-channel-area',
        element: <BackChannelArea />
    }
];

export default AppRoutes;
