module SubscribersHelper
   def find_subscriber_by_project_and_user_id (project, user_id)
     for subscribe in Subscriber.get_all_subscribers do
       if ( project.id == subscribe.project_id && subscribe.user_id.to_s == user_id)
         return subscribe.id
       end
     end
     puts project.id
     return -1     
   end
end
