﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_WPF
{
    using System;
    using System.Collections.Generic;
    
    public class TimeLineData
    {
        public class Relation
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
        }

        public class Actor
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<object> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
            public bool? need_verify { get; set; }
            public string gender_permission { get; set; }
        }

        public class Medium
        {
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string origin_url { get; set; }
            public string url2 { get; set; }
            public string thumbnail_url2 { get; set; }
            public string thumbnail_url3 { get; set; }
            public string jpg_url { get; set; }
            public string key { get; set; }
            public string content_type { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string sq_url { get; set; }
            public string media_path { get; set; }
        }

        public class ContentDecorator
        {
            public string text { get; set; }
            public string type { get; set; }
            public string id { get; set; }
        }

        public class Relation2
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
        }

        public class StatusObject
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Actor2
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation2 relation { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject> status_objects { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Object
        {
            public int comment_count { get; set; }
            public bool downloadable { get; set; }
            public bool pinned { get; set; }
            public DateTime created_at { get; set; }
            public int with_tag_count { get; set; }
            public bool sharable { get; set; }
            public List<Medium> media { get; set; }
            public string content { get; set; }
            public bool required { get; set; }
            public bool liked { get; set; }
            public string sid { get; set; }
            public bool sympathized { get; set; }
            public DateTime updated_at { get; set; }
            public string media_type { get; set; }
            public string activity_type { get; set; }
            public string id { get; set; }
            public bool modifiable { get; set; }
            public bool blinded { get; set; }
            public List<ContentDecorator> content_decorators { get; set; }
            public string summary { get; set; }
            public int like_count { get; set; }
            public string object_type { get; set; }
            public string verb { get; set; }
            public string permission { get; set; }
            public bool comment_all_writable { get; set; }
            public bool has_unread_reaction { get; set; }
            public int share_count { get; set; }
            public Actor2 actor { get; set; }
            public bool push_mute { get; set; }
            public bool deleted { get; set; }
            public bool with_me { get; set; }
            public int sympathy_count { get; set; }
            public string permalink { get; set; }
            public int view_count { get; set; }
        }

        public class Medium2
        {
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string origin_url { get; set; }
            public string url2 { get; set; }
            public string thumbnail_url2 { get; set; }
            public string thumbnail_url3 { get; set; }
            public string jpg_url { get; set; }
            public string key { get; set; }
            public string content_type { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string sq_url { get; set; }
            public string media_path { get; set; }
            public int? height_hq { get; set; }
            public int? size_hq { get; set; }
            public string preview_url_hq { get; set; }
            public int? duration { get; set; }
            public int? width_hq { get; set; }
            public int? size { get; set; }
            public string preview_url { get; set; }
            public string url_hq { get; set; }
            public List<object> caption { get; set; }
        }

        public class TitleDecorator
        {
            public string text { get; set; }
            public string id { get; set; }
            public string type { get; set; }
        }

        public class Scrap
        {
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public string type { get; set; }
            public bool is_opengraph { get; set; }
            public string host { get; set; }
            public string requested_url { get; set; }
            public List<string> image { get; set; }
            public string site_name { get; set; }
            public string section { get; set; }
            public string iid { get; set; }
            public string dest_url { get; set; }
        }

        public class ContentDecorator2
        {
            public string text { get; set; }
            public string type { get; set; }
        }

        public class Relation3
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
        }

        public class StatusObject2
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Actor3
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation3 relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject2> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Activity
        {
            public int comment_count { get; set; }
            public bool downloadable { get; set; }
            public bool pinned { get; set; }
            public string iid { get; set; }
            public bool deletable { get; set; }
            public DateTime created_at { get; set; }
            public int with_tag_count { get; set; }
            public bool sharable { get; set; }
            public string content { get; set; }
            public bool required { get; set; }
            public bool liked { get; set; }
            public string feed_id { get; set; }
            public string sid { get; set; }
            public bool sympathized { get; set; }
            public DateTime updated_at { get; set; }
            public string media_type { get; set; }
            public string activity_type { get; set; }
            public string id { get; set; }
            public bool modifiable { get; set; }
            public List<ContentDecorator2> content_decorators { get; set; }
            public bool blinded { get; set; }
            public List<object> likes { get; set; }
            public int like_count { get; set; }
            public List<object> comments { get; set; }
            public bool bookmarked { get; set; }
            public string verb { get; set; }
            public string permission { get; set; }
            public bool comment_all_writable { get; set; }
            public bool has_unread_reaction { get; set; }
            public int share_count { get; set; }
            public Actor3 actor { get; set; }
            public List<object> latest_friend_emotion { get; set; }
            public bool push_mute { get; set; }
            public bool with_me { get; set; }
            public int sympathy_count { get; set; }
            public List<object> latest_comments { get; set; }
            public int view_count { get; set; }
        }

        public class BundledFeed
        {
            public List<TitleDecorator> title_decorators { get; set; }
            public Scrap scrap { get; set; }
            public List<Activity> activities { get; set; }
            public int total_count { get; set; }
            public bool more_activities { get; set; }
            public string type { get; set; }
        }

        public class Feed
        {
            public int comment_count { get; set; }
            public bool downloadable { get; set; }
            public bool pinned { get; set; }
            public bool is_must_read { get; set; }
            public string iid { get; set; }
            public bool deletable { get; set; }
            public DateTime created_at { get; set; }
            public int with_tag_count { get; set; }
            public bool sharable { get; set; }
            public string content { get; set; }
            public bool required { get; set; }
            public bool liked { get; set; }
            public string feed_id { get; set; }
            public string sid { get; set; }
            public bool sympathized { get; set; }
            public DateTime updated_at { get; set; }
            public string media_type { get; set; }
            public string activity_type { get; set; }
            public string id { get; set; }
            public bool modifiable { get; set; }
            public List<object> content_decorators { get; set; }
            public bool blinded { get; set; }
            public List<object> likes { get; set; }
            public string summary { get; set; }
            public int like_count { get; set; }
            public List<CommentData.Comment> comments { get; set; }
            public bool bookmarked { get; set; }
            public string verb { get; set; }
            public string permission { get; set; }
            public bool comment_all_writable { get; set; }
            public bool has_unread_reaction { get; set; }
            public int share_count { get; set; }
            public Actor actor { get; set; }
            public List<object> latest_friend_emotion { get; set; }
            public bool push_mute { get; set; }
            public bool with_me { get; set; }
            public int sympathy_count { get; set; }
            public List<CommentData.LatestComment> latest_comments { get; set; }
            public string permalink { get; set; }
            public int view_count { get; set; }
            public Object @object { get; set; }
            public List<Medium2> media { get; set; }
            public BundledFeed bundled_feed { get; set; }
            public DateTime? content_updated_at { get; set; }
        }

        public class TimeLine
        {
            public List<CommentData.PostData> feeds { get; set; }
            public string next_since { get; set; }
        }
    }
    public class ProfileData
    {
        public class Relation3
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
            public bool foaf { get; set; }
        }

        public class StatusObject3
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Profile
        {
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public int waiting_question_count { get; set; }
            public bool is_favorite { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation3 relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string friend_accept_level { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public bool message_rejectee { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject3> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public bool message_sendable { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string gender_permission { get; set; }
            public List<string> biography_summary { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class MutualFriend
        {
            public string message { get; set; }
        }
        public class ProfileObject
        {
            public List<CommentData.PostData> activities { get; set; }
            public MutualFriend mutual_friend { get; set; }
            public Profile profile { get; set; }
        }
    }

    public class QuoteData
    {
        public string media_path { get; set; }
        public string type = "text";
        public string text { get; set; }
        public string id { get; set; }
        public CommentMedia media { get; set; }
        public string permalink { get; set; }
        public string hashtag_type;
        public string hashtag_type_id;
    };

    public class FriendInitData
    {
        public class Placeholder
        {
            public string text { get; set; }
            public string type { get; set; }
        }

        public class HashtagSuggestionPlaceholder
        {
            public int placeholder_id { get; set; }
            public int exposure_count { get; set; }
            public string type { get; set; }
            public List<Placeholder> placeholder { get; set; }
            public string desc { get; set; }
            public string desc_en { get; set; }
            public string desc_ja { get; set; }
        }

        public class MediaFiltersNew
        {
            public string filter_id { get; set; }
            public int exposure_count { get; set; }
            public bool tooltip { get; set; }
        }

        public class RnbWithworth
        {
            public bool display { get; set; }
            public string url { get; set; }
            public string label { get; set; }
        }

        public class SearchPlaceholder
        {
            public string gnb_text { get; set; }
            public string gnb_text_en { get; set; }
        }


        public class AppConfig
        {
            public string checksum { get; set; }
        }

        public class Relation
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
        }

        public class StatusObject
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Profile
        {
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public string gender { get; set; }
            public bool need_verify { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string gender_permission { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Relation2
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
        }

        public class Call2action
        {
            public string action_url { get; set; }
            public string install_url { get; set; }
            public string action_format { get; set; }
            public string type { get; set; }
            public string analysis_url { get; set; }
        }

        public class ChannelObject
        {
            public Call2action call2action { get; set; }
            public List<string> keywords { get; set; }
            public List<object> outlinks { get; set; }
            public bool certified { get; set; }
            public string category { get; set; }
            public int follower_count { get; set; }
            public DateTime start_date { get; set; }
        }

        public class Friend
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation2 relation { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public object recently_with_friends_priority { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public bool is_birthday { get; set; }
            public bool message_rejectee { get; set; }
            public string bg_image_url { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<object> status_objects { get; set; }
            public bool message_sendable { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
            public ChannelObject channel_object { get; set; }
            public string profile_video_url_hq { get; set; }
            public string profile_video_url_square_small { get; set; }
            public string profile_video_url_lq { get; set; }
            public string profile_video_url_square { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public bool? message_received_bomb { get; set; }
            public bool? blocked { get; set; }
            public long? message_receiver_priority { get; set; }
        }

        public class FriendData
        {
            public bool authenticated { get; set; }
            public AppConfig appConfig { get; set; }
            public Profile profile { get; set; }
            public bool fromTalk { get; set; }
            public bool has_profile { get; set; }
            public List<Friend> friends { get; set; }
        }
    }
    public class CommentDecorators
    {
        public List<QuoteData> decorators;
    }
    public class ProfileRelationshipData
    {
        public class Relation
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
            public bool foaf { get; set; }
        }

        public class Relation2
        {
            public bool feed_blocked { get; set; }
            public bool self { get; set; }
            public bool favorite { get; set; }
            public bool foaf { get; set; }
        }

        public class FoafList
        {
            public string bg_image_url2 { get; set; }
            public int waiting_question_count { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation2 relation { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string friend_accept_level { get; set; }
            public string id { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public List<object> biography_summary { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public string birthday { get; set; }
            public int? birthday_left { get; set; }
            public string profile_video_url_hq { get; set; }
            public string profile_video_url_square_small { get; set; }
            public string profile_video_url_lq { get; set; }
            public string profile_video_url_square { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
        }

        public class StatusObject
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class ProfileRelationship
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public int waiting_question_count { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public List<FoafList> foaf_list { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string friend_accept_level { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public bool message_rejectee { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public bool message_sendable { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public List<string> biography_summary { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public int foaf_count { get; set; }
            public bool is_feed_blocked { get; set; }
        }
    }
    public class ShareData
    {
        public class Relation
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
        }

        public class Actor
        {
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Share
        {
            public Actor actor { get; set; }
            public string activity_id { get; set; }
            public string type { get; set; }
            public string emotion { get; set; }
            public DateTime created_at { get; set; }
            public string id { get; set; }
        }
    };
    public class CommentData
    {
        public class Medium
        {
            public string url { get; set; }
            public string thumbnail_url { get; set; }
            public string origin_url { get; set; }
            public string url2 { get; set; }
            public string thumbnail_url2 { get; set; }
            public string thumbnail_url3 { get; set; }
            public string jpg_url { get; set; }
            public string key { get; set; }
            public string content_type { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string sq_url { get; set; }
            public string media_path { get; set; }
            public bool searchable { get; set; }
            public string iid { get; set; }
            public string preview_url_hq { get; set; }
            public string url_hq { get; set; }
        }

        public class ContentDecorator
        {
            public string text { get; set; }
            public string type { get; set; }
        }

        public class Decorator
        {
            public string text { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string permalink { get; set; }
        }
        
        public class Relation
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
        }

        public class Writer
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation relation { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<object> status_objects { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool? need_verify { get; set; }
            public string profile_video_url_square_small { get; set; }
            public string profile_video_url_lq { get; set; }
            public string profile_video_url_square { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string birth { get; set; }
            public string gender_permission { get; set; }
        }

        public class Comment
        {
            public int like_count { get; set; }
            public List<QuoteData> decorators { get; set; }
            public string activity_id { get; set; }
            public string index { get; set; }
            public DateTime created_at { get; set; }
            public string id { get; set; }
            public string text { get; set; }
            public Writer writer { get; set; }
            public bool liked { get; set; }
        }

        public class Relation2
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public string ban { get; set; }
            public bool foaf { get; set; }
        }

        public class StatusObject
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Actor
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool need_verify { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation2 relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string friend_accept_level { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string birth { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string gender_permission { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Decorator2
        {
            public string text { get; set; }
            public string type { get; set; }
        }

        public class Relation3
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
        }

        public class StatusObject2
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Writer2
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation3 relation { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject2> status_objects { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class LatestComment
        {
            public int like_count { get; set; }
            public List<Decorator2> decorators { get; set; }
            public string activity_id { get; set; }
            public string index { get; set; }
            public DateTime created_at { get; set; }
            public string id { get; set; }
            public string text { get; set; }
            public Writer2 writer { get; set; }
            public bool liked { get; set; }
        }
        

        public class Relation4
        {
            public bool feed_blocked { get; set; }
            public string friend { get; set; }
            public bool self { get; set; }
            public string follow { get; set; }
            public bool favorite { get; set; }
            public bool foaf { get; set; }
        }

        public class StatusObject4
        {
            public string object_type { get; set; }
            public string message { get; set; }
        }

        public class Actor2
        {
            public string birthday { get; set; }
            public string bg_image_url2 { get; set; }
            public string profile_video_url_hq { get; set; }
            public bool is_favorite { get; set; }
            public int birthday_left { get; set; }
            public string gender { get; set; }
            public bool birth_leap_type { get; set; }
            public string birth_type { get; set; }
            public string profile_image_url2 { get; set; }
            public string type { get; set; }
            public int follower_count { get; set; }
            public Relation4 relation { get; set; }
            public string profile_video_url_square_small { get; set; }
            public bool is_celebratable { get; set; }
            public int default_bg_id { get; set; }
            public int activity_count { get; set; }
            public bool is_valid_user { get; set; }
            public string friend_accept_level { get; set; }
            public string id { get; set; }
            public string relationship { get; set; }
            public bool vip { get; set; }
            public int friend_count { get; set; }
            public string profile_video_url_lq { get; set; }
            public bool is_birthday { get; set; }
            public string bg_image_url { get; set; }
            public string profile_video_url_square { get; set; }
            public bool allow_following { get; set; }
            public string profile_thumbnail_url { get; set; }
            public List<StatusObject4> status_objects { get; set; }
            public string profile_video_url_square_micro_small { get; set; }
            public string profile_image_url { get; set; }
            public string display_name { get; set; }
            public string permalink { get; set; }
            public bool is_default_profile_image { get; set; }
            public bool is_feed_blocked { get; set; }
        }

        public class Object
        {
            public int comment_count { get; set; }
            public bool downloadable { get; set; }
            public bool pinned { get; set; }
            public bool is_must_read { get; set; }
            public DateTime content_updated_at { get; set; }
            public DateTime created_at { get; set; }
            public int with_tag_count { get; set; }
            public bool sharable { get; set; }
            public List<Medium> media { get; set; }
            public string content { get; set; }
            public bool required { get; set; }
            public bool liked { get; set; }
            public string sid { get; set; }
            public bool sympathized { get; set; }
            public DateTime updated_at { get; set; }
            public string media_type { get; set; }
            public string activity_type { get; set; }
            public string id { get; set; }
            public bool modifiable { get; set; }
            public bool blinded { get; set; }
            public List<QuoteData> content_decorators { get; set; }
            public string summary { get; set; }
            public int like_count { get; set; }
            public string object_type { get; set; }
            public string verb { get; set; }
            public string permission { get; set; }
            public TimeLineData.Scrap scrap { get; set; }
            public bool comment_all_writable { get; set; }
            public bool has_unread_reaction { get; set; }
            public int share_count { get; set; }
            public Actor2 actor { get; set; }
            public bool push_mute { get; set; }
            public bool deleted { get; set; }
            public bool with_me { get; set; }
            public int sympathy_count { get; set; }
            public string permalink { get; set; }
            public int view_count { get; set; }
        }

        public class PostData
        {
            public int comment_count { get; set; }
            public bool downloadable { get; set; }
            public bool pinned { get; set; }
            public bool is_must_read { get; set; }
            public bool deletable { get; set; }
            public bool deleted { get; set; }
            public TimeLineData.Scrap scrap { get; set; }
            public DateTime created_at { get; set; }
            public int with_tag_count { get; set; }
            public bool sharable { get; set; }
            public List<Medium> media { get; set; }
            public string content { get; set; }
            public bool required { get; set; }
            public bool liked { get; set; }
            public string feed_id { get; set; }
            public string sid { get; set; }
            public bool sympathized { get; set; }
            public string next_call_for_activity_recommend { get; set; }
            public DateTime content_updated_at { get; set; }
            public DateTime updated_at { get; set; }
            public string media_type { get; set; }
            public string activity_type { get; set; }
            public string id { get; set; }
            public bool modifiable { get; set; }
            public List<QuoteData> content_decorators { get; set; }
            public bool blinded { get; set; }
            public List<ShareData.Share> likes { get; set; }
            public string summary { get; set; }
            public int like_count { get; set; }
            public List<Comment> comments { get; set; }
            public bool bookmarked { get; set; }
            public string verb { get; set; }
            public string permission { get; set; }
            public bool comment_all_writable { get; set; }
            public bool has_unread_reaction { get; set; }
            public int share_count { get; set; }
            public Actor actor { get; set; }
            public List<object> latest_friend_emotion { get; set; }
            public bool push_mute { get; set; }
            public bool with_me { get; set; }
            public int sympathy_count { get; set; }
            public List<LatestComment> latest_comments { get; set; }
            public string permalink { get; set; }
            public int view_count { get; set; }
            public Object @object { get; set; }
        }
    }

    public class CommentMedia
    {
        public int height { get; set; }
        public int width { get; set; }
        public string media_path { get; set; }
        public string thumbnail_url { get; set; }
        public string url { get; set; }
        public string origin_url { get; set; }
    }

    public class Relation
    {
        public bool feed_blocked { get; set; }
        public string friend { get; set; }
        public bool self { get; set; }
        public string follow { get; set; }
        public bool favorite { get; set; }
    }

    public class Actor
    {
        public string birthday { get; set; }
        public bool is_favorite { get; set; }
        public int birthday_left { get; set; }
        public string gender { get; set; }
        public bool birth_leap_type { get; set; }
        public string birth_type { get; set; }
        public string type { get; set; }
        public int follower_count { get; set; }
        public Relation relation { get; set; }
        public bool is_celebratable { get; set; }
        public int default_bg_id { get; set; }
        public int activity_count { get; set; }
        public bool is_valid_user { get; set; }
        public string id { get; set; }
        public string relationship { get; set; }
        public bool vip { get; set; }
        public int friend_count { get; set; }
        public bool is_birthday { get; set; }
        public string bg_image_url { get; set; }
        public bool allow_following { get; set; }
        public string profile_thumbnail_url { get; set; }
        public string profile_image_url { get; set; }
        public string display_name { get; set; }
        public string permalink { get; set; }
        public bool is_default_profile_image { get; set; }
        public bool is_feed_blocked { get; set; }
        public string profile_video_url_hq { get; set; }
        public string profile_video_url_square_small { get; set; }
        public string profile_video_url_lq { get; set; }
        public string profile_video_url_square { get; set; }
        public string profile_video_url_square_micro_small { get; set; }
    }

    public class Decorator
    {
        public string text { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }

    public class CSNotification
    {
        public Actor actor { get; set; }
        public string scheme { get; set; }
        public bool is_new { get; set; }
        public List<Decorator> decorators { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public string thumbnail_url { get; set; }
        public string thumbnail_text { get; set; }
        public string key { get; set; }
        public string content { get; set; }
        public string comment_id { get; set; }
        public string like_id { get; set; }
        public string emotion { get; set; }
    }
}
