json.array!(@subscribers) do |subscriber|
  json.extract! subscriber, :id,:product_id, :user_id, :create_time, :description, :subscriber_type
  json.url subscriber_url(subscriber, format: :json)
end
