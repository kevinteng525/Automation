json.array!(@users) do |user|
  json.extract! user, :name, :email,:password, :user_type, :role, :is_active, :description
  json.url user_url(user, format: :json)
end
