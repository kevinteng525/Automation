class Subscriber < ActiveRecord::Base
  attr_accessor :project_id
  private
  def Subscriber.initialize_from_json_params(params)
    subscriber = Subscriber.new    
    subscriber.id = params['SubscriberId']
    subscriber.project_id = params['ProjectId']
    subscriber.user_id = params['UserId']
    subscriber.create_time = ms_json_to_date(params['CreateTime'])
    subscriber.description = params['Description']
    subscriber.subscriber_type = params['SubscriberType']
    subscriber
  end
  def Subscriber.serialize_params_to_json(params)
    hash = Hash.new
    hash[:ProjectId] = params['project_id']
    hash[:UserId] = params['user_id']
    hash[:CreateTime] = params['create_time']
    hash[:SubscriberType] = params['subscriber_type']
    hash[:Description] = params['description']     
    JSON.generate(hash)    
  end
  def Subscriber.get_all_subscribers()
    list = Array.new    
    response = force_server['galaxyglobal/subscriber'].get
    if response!=''
      params = JSON.parse(response.body)
      params.each do |param|
        list.push Subscriber.initialize_from_json_params(param)      
      end
    end
    list
  end
  def Subscriber.create_force_subscriber(params)   
    response = force_server['galaxyglobal/subscriber'].post Subscriber.serialize_params_to_json(params),:content_type => 'application/json', :accept => 'application/json'
    Subscriber.initialize_from_json_params(JSON.parse(response.body))
  end
   def Subscriber.delete_force_subscriber(id)
    response = force_server['galaxyglobal/subscriber/' + id.to_s].delete
  end  
  def Subscriber.get_force_subscriber_by_id(id)
    response = force_server['galaxyglobal/subscriber/' + id.to_s].get
    Subscriber.initialize_from_json_params(JSON.parse(response.body))
  end
end
